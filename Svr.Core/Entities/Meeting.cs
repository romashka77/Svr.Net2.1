using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Заседание
    /// </summary>
    public class Meeting : BaseEntityDescription
    {
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
        /// Номер
        /// </summary>
        [Display(Name = "Номер")]
        public byte Number { get; set; }
        /// <summary>
        /// Дата
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Дата")]
        public DateTime? Date { get; set; }
        /// <summary>
        /// Время
        /// </summary>
        [DataType(DataType.Time)]
        [Display(Name = "Время")]
        public DateTime? Time { get; set; }
        public override string ToString() => "Заседание";
    }
}
