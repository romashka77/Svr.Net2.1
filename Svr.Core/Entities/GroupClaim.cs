using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Группа исков
    /// </summary>
    public class GroupClaim : BaseEntityCode
    {
        /// <summary>
        /// Id Категории споров
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long CategoryDisputeId { get; set; }
        /// <summary>
        /// Категория споров
        /// </summary>
        [Display(Name = "Категория споров")]
        //[ForeignKey("CategoryDisputeId")]
        public virtual CategoryDispute CategoryDispute { get; set; }  //навигационное свойство

        /// <summary>
        /// Предметы исков
        /// </summary>
        [Display(Name = "Предметы исков")]
        public virtual ICollection<SubjectClaim> SubjectClaims { get; set; } = new List<SubjectClaim>();

        public override string ToString() => "Группа исков";
    }
}
