using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        const string str = "Пожалуйста, заполните поле: {0}";
        [EmailAddress(ErrorMessage = "Пожалуйста, проверьте {0}")]
        [Required(ErrorMessage = str)]
        [Display(Name = "E-mail", Description = "Email Адрес", Prompt = "Введите E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = str)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }
}
