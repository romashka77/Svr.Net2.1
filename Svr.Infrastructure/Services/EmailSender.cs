using MailKit.Net.Smtp;
using MimeKit;
using Svr.Core.Interfaces;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Администрация сайта ПК «Судебная практика»", "079-0824@079.pfr.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.To.Add(new MailboxAddress("", "079-0824@079.pfr.ru"));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            var client = new SmtpClient();
            await client.ConnectAsync("s079.079.pfr.ru").ConfigureAwait(false);
            //await client.AuthenticateAsync("079-0824", "1234");
            await client.SendAsync(emailMessage).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
        }
    }
}
