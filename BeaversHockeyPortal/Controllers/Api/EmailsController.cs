using DataModel;
using DataModel.Repositories;
using EmailModule;
using LanguageParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;

namespace BeaversHockeyPortal.Controllers.Api
{
    public class EmailsController : ApiController
    {
        private enum EmailSendingStatus
        {
            None = 0,
            Failed,
            Successful,
            AlreadySent,
            NoGameForEmail,
            TestOnlyDoNotSend,
        }

        private class EmailLogEntry
        {
            public int EmailEventId { get; set; }
            public int? DaysBeforeGame { get; set; }
            public int? Reoccurrence { get; set; }

            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }

            public string EmailEventType { get; set; }

            public DateTime? GameDate { get; set; }

            public string GameName { get; set; }
            public string Message { get; set; }

            public string EmailSendingStatus { get; set; }

            public EmailLogEntry Clone()
            {
                return new EmailLogEntry
                {
                    EmailEventId = this.EmailEventId,
                    DaysBeforeGame = this.DaysBeforeGame,
                    Reoccurrence = this.Reoccurrence,
                    To = this.To,
                    Subject = this.Subject,
                    Body = this.Body,
                    EmailEventType = this.EmailEventType,
                    GameDate = this.GameDate,
                    GameName = this.GameName,
                    Message = this.Message,
                };
            }
        }

        private DataModel.DataModelContext _ctx;
        private IEmailSender _emailSender;
        private ILanguageParser _languageParser;

        private List<EmailLogEntry> _status = new List<EmailLogEntry>();

        public EmailsController(DataModelContext ctx, IEmailSender emailSender, ILanguageParser languageParser)
        {
            this._ctx = ctx;
            this._emailSender = emailSender;
            this._languageParser = languageParser;
        }

        // GET: api/EmailSender
        [Route("api/Emails/jacks")]
        public IHttpActionResult Get()
        {
            try
            {
                return Ok(this._ctx.EmailEvents.Include(e => e.EmailTemplates).Include(e => e.EmailEventTypes));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private DateTime Today
        {
            get { return DateTime.Now.Date; }
        }

        [Route("api/Emails/Send")]
        public IHttpActionResult Send(int id)
        {
            var emailEvent = this._ctx.EmailEvents.FirstOrDefault(e => e.Id == id);

            if (emailEvent == null)
            {
                return BadRequest($"Invalid email-event id: {id}");
            }
            else
            {
                this.ProcessEmailEvent(emailEvent, false);

                return Ok();
            }
        }

        // GET api/EmailSender/TestEmailsToSend
        [Route("api/Emails/EmailLogs")]
        [HttpGet]
        public IHttpActionResult EmailLogs()
        {
            try
            {
                return Json(this._ctx.EmailLogs
                    .Include(log => log.Game)
                    .Include(log => log.EmailEvent)
                    .Include(log => log.EmailEvent.EmailEventTypes)
                    .Include(log => log.EmailEvent.EmailTemplates)
                    .OrderByDescending(log => log.TimeStamp)
                    .Take(100));
            }
            catch (Exception ex)
            {
                return BadRequest($"Fatal error getting email logs: {ex.Message}");
            }
        }

        // GET api/EmailSender/TestEmailsToSend
        [Route("api/Emails/TestEmailsToSend")]
        [HttpGet]
        public IHttpActionResult TestEmailsToSend()
        {
            return this.ProcessAllEmails(true);
        }

        // GET api/EmailSender/SendAll
        [Route("api/Emails/Send")]
        [HttpGet]
        public IHttpActionResult Send()
        {
            return this.ProcessAllEmails(false);
        }

        private IHttpActionResult ProcessAllEmails(bool isTestOnly)
        {
            try
            {
                this._ctx.EmailEvents
    .Include(e => e.EmailTemplates)
    .Include(e => e.EmailEventTypes)
    .Include(e => e.Manager)
    .ToList()
    .ForEach(emailEvent => this.ProcessEmailEvent(emailEvent, isTestOnly));

                return Json(this._status);
                //Ok("Successfully sent out emails");
            }
            catch (Exception ex)
            {
                return BadRequest($"Fatal error sending emails: {ex.Message}");
            }
        }

        private void ProcessEmailEvent(EmailEvent emailEvent, bool isTestOnly)
        {
            var gameEmailEvent = emailEvent as GameEmailEvent;

            var emailLogEntry = new EmailLogEntry
            {
                EmailEventId = emailEvent.Id,
                DaysBeforeGame = null,
                Reoccurrence = null,
                EmailEventType = "Other",
                GameDate = null,
                GameName = "N/A",
            };

            if (gameEmailEvent != null)
            {
                emailLogEntry.DaysBeforeGame = gameEmailEvent.DaysBeforeGame;
                emailLogEntry.Reoccurrence = gameEmailEvent.DaysForReoccurrence;

                this.ProcessGameEmailEvent(gameEmailEvent, emailLogEntry, isTestOnly);
            }
            else
            {
                this.ProcessGenericEmailEvent(emailEvent, emailLogEntry, isTestOnly);
            }
        }

        private void ProcessGenericEmailEvent(EmailEvent emailEvent, EmailLogEntry emailLogEntry, bool isTestOnly)
        {
            try
            {
                // Proceed only if email not sent, or it's a reccurring email
                var emailLog = this._ctx.EmailLogs.FirstOrDefault(el => el.EmailEvent.Id == emailEvent.Id && el.Successful && el.TimeStamp == this.Today);

                var emailSendingStatus = EmailSendingStatus.None;
                if (isTestOnly)
                {
                    emailSendingStatus = EmailSendingStatus.TestOnlyDoNotSend;
                }
                else if (emailLog != null)
                {
                    emailSendingStatus = EmailSendingStatus.AlreadySent;
                }

                var sendEmails = !isTestOnly && emailLog == null;

                // Process each email template for the Email Event
                SendAndSaveEmails(emailEvent, emailLogEntry, sendEmails, emailSendingStatus, null);

                this._ctx.SaveChanges();

            }
            catch (Exception ex)
            {
                emailLogEntry.EmailSendingStatus = EmailSendingStatus.Failed.ToString();
                emailLogEntry.Message = ex.Message;
                this._status.Add(emailLogEntry);

                LoggingModule.Logger.Instance.LogError($"Error sending email for email event: {emailEvent.Id}, event type: OTHER", ex);
            }
        }

        private void ProcessGameEmailEvent(GameEmailEvent emailEvent, EmailLogEntry emailLogEntry, bool isTestOnly)
        {
            var dateBeforeGame = this.Today.AddDays(emailEvent.DaysBeforeGame);

            foreach (var emailEventType in emailEvent.EmailEventTypes)
            {
                emailLogEntry.EmailEventType = emailEventType.Name;

                try
                {
                    var game = this._ctx.Games.OrderBy(g => g.Date)
                         .Include(g => g.Manager)
                         .Include(g => g.Us)
                         .Include(g => g.Them)
                         .FirstOrDefault(g => g.Date <= dateBeforeGame);

                    if (game == null)
                    {
                        emailLogEntry.EmailSendingStatus = EmailSendingStatus.NoGameForEmail.ToString();
                        this._status.Add(emailLogEntry);

                        LoggingModule.Logger.Instance.LogError($"There must be a game to send emails for Game Email Event Id: {emailEvent.Id}", ex: null);
                        continue;
                    }

                    emailLogEntry.GameDate = game.Date;
                    emailLogEntry.GameName = game.ToString();

                    // Proceed only if email not sent, or it's a reccurring email
                    var emailLog = this._ctx.EmailLogs.FirstOrDefault(el => el.Game != null &&
                                                                                           el.Game.Id == game.Id &&
                                                                                           el.EmailEvent.Id == emailEvent.Id &&
                                                                                           el.Successful);

                    var emailAlreadySentForGame = emailLog != null;

                    var needToSendEmail = !emailAlreadySentForGame;

                    // check Re-Occurrence
                    if (emailAlreadySentForGame && emailEvent.DaysForReoccurrence.HasValue)
                    {
                        var resendOn = emailLog.TimeStamp.Date.AddDays(emailEvent.DaysForReoccurrence.Value);

                        needToSendEmail = this.Today == resendOn;
                    }

                    EmailSendingStatus emailSendingStatus = EmailSendingStatus.None;

                    if (isTestOnly)
                    {
                        emailSendingStatus = EmailSendingStatus.TestOnlyDoNotSend;
                    }
                    else if (!needToSendEmail)
                    {
                        emailSendingStatus = EmailSendingStatus.AlreadySent;
                    }

                    // Process each email template for the Email Event
                    SendAndSaveEmails(emailEvent, emailLogEntry, !isTestOnly && needToSendEmail, emailSendingStatus, game);

                    this._ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    emailLogEntry.EmailSendingStatus = EmailSendingStatus.Failed.ToString();
                    emailLogEntry.Message = ex.Message;
                    this._status.Add(emailLogEntry);

                    LoggingModule.Logger.Instance.LogError($"Error sending email for email event: {emailEvent.Id}, event type: {emailEventType.Name}", ex);
                }
            }
        }

        private void SendAndSaveEmails(EmailEvent emailEvent, EmailLogEntry emailLogEntry, bool sendEmails, EmailSendingStatus emailSendingStatus, Game game = null)
        {
            foreach (var emailTemplate in emailEvent.EmailTemplates)
            {
                if (sendEmails)
                {
                    bool sendingEmailSucceeded = this.SendEmailsForTemplate(emailTemplate, emailEvent.Id);

                    emailSendingStatus = sendingEmailSucceeded ? EmailSendingStatus.Successful : EmailSendingStatus.Failed;

                    // Log email entry
                    this._ctx.EmailLogs.Add(new EmailLog
                    {
                        EmailEvent = emailEvent,
                        Game = game,
                        Successful = sendingEmailSucceeded,
                        TimeStamp = this.Today,
                    });
                }

                var emailTemplateLogEntry = emailLogEntry.Clone();

                emailTemplateLogEntry.EmailSendingStatus = emailSendingStatus.ToString();
                emailTemplateLogEntry.To = emailTemplate.To;
                emailTemplateLogEntry.Subject = emailTemplate.Subject;
                emailTemplateLogEntry.Body = emailTemplate.Body;

                this._status.Add(emailTemplateLogEntry);
            }
        }

        private bool SendEmailsForTemplate(EmailTemplate emailTemplate, int emailEventId)
        {
            bool emailSucceeded = true;
            try
            {
                var to = this.GetTo(emailTemplate);
                var body = this._languageParser.ParseForManager(emailTemplate.Body, emailTemplate.Context, emailTemplate.Manager.Id, emailTemplate.AggrigateLanguageResults);
                var subject = this._languageParser.ParseForManager(emailTemplate.Subject, emailTemplate.Context, emailTemplate.Manager.Id, emailTemplate.AggrigateLanguageResults);

                var validEmailParsing = emailSucceeded =
                    to.Count() == 1 ||
    (to.Count() == body.Count() || body.Count() == 1) ||
    (to.Count() == subject.Count() || subject.Count() == 1);

                if (validEmailParsing)
                {
                    for (int i = 0; i < to.Count(); i++)
                    {
                        emailSucceeded = this._emailSender.SendEmail(emailTemplate.From,
                            to.Count() == 1 ? to[0] : to[i],
                            subject.Count() == 1 ? subject[0] : subject[i],
                            body.Count() == 1 ? body[0] : body[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingModule.Logger.Instance.LogError($"Error sending an email (event id: {emailEventId})", ex);
                emailSucceeded = false;
            }

            return emailSucceeded;
        }

        private List<string> GetTo(EmailTemplate emailTemplate)
        {
            if (emailTemplate.SendToSpecificUsers)
            {
                throw new NotImplementedException("Not implemeted sending emailt to predefinied user list");
            }
            else
            {
                return this._languageParser.ParseForManager(emailTemplate.To, emailTemplate.Context, emailTemplate.Manager.Id, emailTemplate.AggrigateLanguageResults);
            }

        }

    }
}
