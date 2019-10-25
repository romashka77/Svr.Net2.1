using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail", Description = "Email Адрес", Prompt = "Введите E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля", Prompt = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
