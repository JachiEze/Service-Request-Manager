using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ServiceRequestForm.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendLoginNotification(string toEmail)
        {
            var smtpHost = _config["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
            var smtpUser = _config["EmailSettings:SmtpUser"];
            var smtpPass = _config["EmailSettings:SmtpPass"];

            var message = new MailMessage();
            message.From = new MailAddress(smtpUser);
            message.To.Add(toEmail);
            message.Subject = "New Login Notification";
            message.Body = $"Hello,\n\nYour account on the Service Request Management Application was just logged in at {DateTime.UtcNow} UTC.\n\nIf this wasn't you, please secure your account.";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            client.Send(message);
        }
    }
}
