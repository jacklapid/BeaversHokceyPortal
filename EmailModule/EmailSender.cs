using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using SendGrid;

namespace EmailModule
{
    public class EmailSender : IEmailSender
    {
        public bool SendEmail(string from, string to, string subject, string body)
        {
            try
            {
                var myMessage = new SendGridMessage();

                // Add the message properties.
                myMessage.From = new MailAddress(from);

                myMessage.AddTo(to);

                myMessage.Subject = subject;

                //Add the HTML and Text bodies
                myMessage.Html = body;
                myMessage.Text = body;

                // SENDGRID_APIKEY: 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
                // SENDGRID_USER: jack
                // SENDGRID_PASS : jackjack 

                //var username = System.Environment.GetEnvironmentVariable("SENDGRID_USER");
                //var pswd = System.Environment.GetEnvironmentVariable("SENDGRID_PASS");

                var apiKey = Utilities.WebSiteSettings.GetSettingValue<string>("SENDGRID_APIKEY");

                //apiKey = "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";

                // create a Web transport, using API Key
                var transportWeb = new Web(apiKey);

                // Send the email, which returns an awaitable task.
                transportWeb.DeliverAsync(myMessage);
            }
            catch(Exception ex)
            {
                LoggingModule.Logger.Instance.LogError($"Error Sending the email to: {to}", ex);

                return false;
            }

            LoggingModule.Logger.Instance.LogInfo($"Successfully sent email to: {to}");

            return true;
        }
    }
}
