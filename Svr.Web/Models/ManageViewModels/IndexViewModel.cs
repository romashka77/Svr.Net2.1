using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "Логин")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-mail", Prompt = "Введите E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Имя", Prompt = "Введите имя")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Фамилия", Prompt = "Введите фамилию")]
        public string LastName { get; set; }

        [StringLength(100)]
        [Display(Name = "Отчество", Prompt = "Введите отчество")]
        public string MiddleName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime DateofBirth { get; set; }

        [Phone]
        [Display(Name = "Номер телефона", Prompt = "Введите номер телефона")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
