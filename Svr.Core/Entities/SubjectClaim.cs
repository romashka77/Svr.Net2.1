using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Предмет иска
    /// </summary>
    public class SubjectClaim : BaseEntityCode
    {
        /// <summary>
        /// Id Группы исков
        /// </summary>
        public long GroupClaimId { get; set; }
        /// <summary>
        /// Группа исков
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        [Display(Name = "Группа исков")]
        //[ForeignKey("GroupClaimId")]
        public virtual GroupClaim GroupClaim { get; set; }  //навигационное свойство
        /// <summary>
        /// Коллекция предметов исков
        /// </summary>
        [Display(Name = "Предметы исков")]
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

        public override string ToString() => "Предмет иска";
    }
}
