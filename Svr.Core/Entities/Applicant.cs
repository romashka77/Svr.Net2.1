using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Сторона процесса
    /// </summary>
    public class Applicant : BaseEntityDescription
    {
        /// <summary>
        /// Тип контрагента
        /// </summary>
        //[Required(ErrorMessage = ErrorStringEmpty)]
        [ForeignKey("TypeApplicant"), Column(Order = 0)]
        public long? TypeApplicantId { get; set; }
        /// <summary>
        /// Id ОПФ
        /// </summary>
        [ForeignKey("Opf"), Column(Order = 1)]
        public long? OpfId { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? Born { get; set; }
        /// <summary>
        /// Полное наименование
        /// </summary>
        [Display(Name = "Полное наименование", Prompt = "Введите полное наименование")]
        public string FullName { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        [Display(Name = "Адрес", Prompt = "Введите адрес")]
        public string Address { get; set; }
        /// <summary>
        /// Адрес банка
        /// </summary>
        [Display(Name = "Адрес банка", Prompt = "Введите адрес")]
        public string AddressBank { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        [Display(Name = "ИНН", Prompt = "Введите ИНН")]
        public string Inn { get; set; }
        /// <summary>
        /// Тип контрагента
        /// </summary>
        [Display(Name = "Тип контрагента")]
        public virtual Dir TypeApplicant { get; set; }
        /// <summary>
        /// ОПФ
        /// </summary>
        [Display(Name = "ОПФ")]
        public virtual Dir Opf { get; set; }

        public override string ToString() => "Сторона процесса";
    }
}
