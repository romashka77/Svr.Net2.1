using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Названия справочников
    /// </summary>
    public class DirName : BaseEntityName
    {
        /// <summary>
        /// Список справочников
        /// </summary>
        [Display(Name = "Справочник")]
        public virtual ICollection<Dir> Dirs { get; set; } = new List<Dir>();

        public override string ToString() => "Названия справочников";
    }
}
