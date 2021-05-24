using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailSender(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }
        
        public async Task SendAsync(NotificationType notificationType, DateTime date)
        {
            var smtpClient = new SmtpClient(_emailConfiguration.Server)
            {
                Port = _emailConfiguration.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailConfiguration.Username, _emailConfiguration.Password),
                EnableSsl = _emailConfiguration.UseSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_emailConfiguration.FromAddress, _emailConfiguration.FromName),
                To = { new MailAddress(_emailConfiguration.To)},
                Subject = EmailDataProvider.GetEmailTitle(notificationType),
                Body = EmailDataProvider.GetEmailContent(_emailConfiguration.AppEndpoint, notificationType, date).PlainText
            };
            
            await smtpClient.SendMailAsync(mail);
        }
    }
}