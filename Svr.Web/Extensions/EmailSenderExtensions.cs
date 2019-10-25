using Svr.Core.Interfaces;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Svr.Web.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Проверка адреса электронной почты", $"Пожалуйста, подтвердите свой аккаунт, перейдя по этой ссылке: {HtmlEncoder.Default.Encode(link)}");
            //$"Пожалуйста, подтвердите свой аккаунт, перейдя по этой ссылке: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
