using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль", Prompt = "Введите новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля", Prompt = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и пароль подтверждения не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
