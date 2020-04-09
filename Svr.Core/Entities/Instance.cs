using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Инстанция
    /// </summary>
    public class Instance : BaseEntityDescription
    {
        /// <summary>
        /// Номер инстанции
        /// </summary>
        [Display(Name = "Номер инстанции")]
        public byte Number { get; set; }
        /// <summary>
        /// Id Иска
        /// </summary>
        public long ClaimId { get; set; }
        /// <summary>
        /// Иск
        /// </summary>
        [Display(Name = "Иск")]
        public virtual Claim Claim { get; set; }
        /// <summary>
        /// Дата передачи
        /// </summary>
        [Display(Name = "Дата передачи")]
        [DataType(DataType.Date)]
        public DateTime? DateTransfer { get; set; }
        /// <summary>
        /// Id Решения суда
        /// </summary>
        public long? CourtDecisionId { get; set; }
        /// <summary>
        /// Решение суда
        /// </summary>
        [Display(Name = "Решение суда")]
        [ForeignKey("CourtDecisionId")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public virtual Dir CourtDecision { get; set; }
        /// <summary>
        /// Дата решения
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Дата решения")]
        public DateTime? DateCourtDecision { get; set; }
        /// <summary>
        /// Дата получения решения
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Дата получения решения")]
        public DateTime? DateInCourtDecision { get; set; }
        /// <summary>
        /// Сумма отказано
        /// </summary>
        [Display(Name = "Сумма отказано")]
        [Column(TypeName = "money")]
        public decimal? SumDenied { get; set; }
        /// <summary>
        /// Сумма удовлетворено
        /// </summary>
        [Display(Name = "Сумма удовлетворено")]
        [Column(TypeName = "money")]
        public decimal? SumSatisfied { get; set; }

        //[Display(Name = "Уплачено добровольно")]
        //[Column(TypeName = "money")]
        //public decimal? PaidVoluntarily { get; set; }
        /// <summary>
        /// Гос.пошлина удов.
        /// </summary>
        [Display(Name = "Гос.пошлина удов.")]
        [Column(TypeName = "money")]
        public decimal? DutySatisfied { get; set; }
        /// <summary>
        /// Гос.пошлина отк.
        /// </summary>
        [Display(Name = "Гос.пошлина отк.")]
        [Column(TypeName = "money")]
        public decimal? DutyDenied { get; set; }
        /// <summary>
        /// Услуги пред.удов.
        /// </summary>
        [Display(Name = "Услуги пред.удов.")]
        [Column(TypeName = "money")]
        public decimal? ServicesSatisfied { get; set; }
        /// <summary>
        /// Услуги пред.отк.
        /// </summary>
        [Display(Name = "Услуги пред.отк.")]
        [Column(TypeName = "money")]
        public decimal? ServicesDenied { get; set; }
        /// <summary>
        /// Суд.издер.удов.
        /// </summary>
        [Display(Name = "Суд.издер.удов.")]
        [Column(TypeName = "money")]
        // ReSharper disable once IdentifierTypo
        public decimal? СostSatisfied { get; set; }
        /// <summary>
        /// Суд.издер.отк.
        /// </summary>
        [Display(Name = "Суд.издер.отк.")]
        [Column(TypeName = "money")]
        // ReSharper disable once IdentifierTypo
        public decimal? СostDenied { get; set; }
        /// <summary>
        /// Упл.гос.пошлина
        /// </summary>
        [Display(Name = "Упл.гос.пошлина")]
        [Column(TypeName = "money")]
        public decimal? DutyPaid { get; set; }

        public override string ToString() => "Инстанция";
    }
}
