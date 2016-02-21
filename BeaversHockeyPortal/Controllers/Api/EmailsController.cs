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
    [Authorize]
    public class EmailsController : ApiController
    {
        private enum EmailSendingStatus
        {
            Failed,
            Successful,
            AlreadySent,
            NoGameForEmail,
        }

        private class EmailLogEntry
        {
            public int Id { get; set; }
            public int DaysBeforeGame { get; set; }
            public int Reoccurrence { get; set; }

            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }

            public string EmailEvent { get; set; }

            public DateTime GameDate { get; set; }

            public string GameName { get; set; }
            public string Message { get; set; }

            public string EmailSendingStatus { get; set; }

            public EmailLogEntry Clone()
            {
                return new EmailLogEntry
                {
                    Id = this.Id,
                    DaysBeforeGame = this.DaysBeforeGame,
                    Reoccurrence = this.Reoccurrence,
                    To = this.To,
                    Subject = this.Subject,
                    Body = this.Body,
                    EmailEvent = this.EmailEvent,
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
                this.ProcessEmailEvent(emailEvent);

                return Ok();
            }
        }

        // GET api/EmailSender/SendAll
        [Route("api/Emails/Send")]
        [HttpGet]
        public IHttpActionResult Send()
        {
            try
            {
                this._ctx.EmailEvents
    .Include(e => e.EmailTemplates)
    .Include(e => e.EmailEventTypes)
    .ToList()
    .ForEach(emailEvent => this.ProcessEmailEvent(emailEvent));

                return Json(this._status);
                //Ok("Successfully sent out emails");
            }
            catch (Exception ex)
            {
                return BadRequest($"Fatal error sending emails: {ex.Message}");
            }

        }

        private void ProcessEmailEvent(GameEmailEvent emailEvent)
        {
            var dateBeforeGame = this.Today.AddDays(emailEvent.DaysBeforeGame);

            foreach (var emailEventType in emailEvent.EmailEventTypes)
            {
                var emailLogEntry = new EmailLogEntry
                {
                    Id = emailEvent.Id,
                    DaysBeforeGame = emailEvent.DaysBeforeGame,
                    Reoccurrence = emailEvent.DaysForReoccurrence ?? 0,
                    EmailEvent = emailEventType.Name,
                };

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

                    if (!needToSendEmail)
                    {
                        emailLogEntry.EmailSendingStatus = EmailSendingStatus.AlreadySent.ToString();
                        this._status.Add(emailLogEntry);

                        continue;
                    }

                    // Process each email template for the Email Event
                    foreach (var emailTemplate in emailEvent.EmailTemplates)
                    {
                        bool sendingEmailSucceeded = this.SendEmailsForTemplate(emailTemplate, emailEvent.Id);

                        var emailTemplateLogEntry = emailLogEntry.Clone();

                        emailTemplateLogEntry.EmailSendingStatus = sendingEmailSucceeded ? EmailSendingStatus.Successful.ToString() : EmailSendingStatus.Failed.ToString();
                        emailTemplateLogEntry.To = emailTemplate.To;
                        emailTemplateLogEntry.Subject = emailTemplate.Subject;
                        emailTemplateLogEntry.Body = emailTemplate.Body;
                        this._status.Add(emailTemplateLogEntry);

                        // Log email entry
                        this._ctx.EmailLogs.Add(new EmailLog
                        {
                            EmailEvent = emailEvent,
                            Game = game,
                            Successful = sendingEmailSucceeded,
                            TimeStamp = this.Today,
                        });
                    }

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

        private bool SendEmailsForTemplate(EmailTemplate emailTemplate, int emailEventId)
        {
            bool emailSucceeded = true;
            try
            {
                var to = this.GetTo(emailTemplate);
                var body = this._languageParser.ParseForManager(emailTemplate.Body, emailTemplate.Context, emailTemplate.Manager.Id);
                var subject = this._languageParser.ParseForManager(emailTemplate.Subject, emailTemplate.Context, emailTemplate.Manager.Id);

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
                return this._languageParser.ParseForManager(emailTemplate.To, emailTemplate.Context, emailTemplate.Manager.Id);
            }

        }

    }
}
