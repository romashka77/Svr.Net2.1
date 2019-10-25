using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Категория споров
    /// </summary>
    public class CategoryDispute : BaseEntityDescription
    {
        /// <summary>
        /// Коллекция Групп исков
        /// </summary>
        [Display(Name = "Группы исков")]
        public virtual ICollection<GroupClaim> GroupClaims { get; set; } = new List<GroupClaim>();
        public override string ToString() => "Категория споров";
    }
}
