using Svr.Core.Interfaces;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Svr.Web.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "�������� ������ ����������� �����", $"����������, ����������� ���� �������, ������� �� ���� ������: {HtmlEncoder.Default.Encode(link)}");
            //$"����������, ����������� ���� �������, ������� �� ���� ������: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
