using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Человек
    /// </summary>
    public class Man : BaseEntity
    {
        [Required(ErrorMessage = ErrorStringEmpty)]
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "Фамилия", Prompt = "Введите фамилию")]
        public string LastName { get; set; }//Фамилия

        [Required(ErrorMessage = ErrorStringEmpty)]
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "Имя", Prompt = "Введите имя")]
        public string FirstName { get; set; }//Имя

        [Display(Name = "Отчество", Prompt = "Введите отчество")]
        public string MiddleName { get; set; }//Отчество

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateofBirth { get; set; }//Дата рождения

        [Display(Name = "СНИЛС", Prompt = "Введите СНИЛС")]
        [Range(typeof(uint), "0", "999999999", ErrorMessage = "Значение {0} должно быть в диапазоне от {1} до {2}")]
        public uint? Snils { get; set; }
        /// <summary>
        /// Район
        /// </summary>
        public long DistrictId { get; set; }
        [Display(Name = "Район", Prompt = "Выберите район")]
        public virtual District District { get; set; }  // навигационное свойство
        //[NotMapped]//чтобы не создавался столбец в таблице.
        //public byte? SnilsChecksum { get; }
        public override string ToString() => "Человек";
    }
}
