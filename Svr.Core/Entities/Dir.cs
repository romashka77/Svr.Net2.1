using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Справочник
    /// </summary>
    public class Dir : BaseEntityName
    {
        /// <summary>
        /// Id Наименования справочника
        /// </summary>
        public long DirNameId { get; set; }
        /// <summary>
        /// Наименование справочника
        /// </summary>
        [Display(Name = "Наименование справочника")]
        public virtual DirName DirName { get; set; }  // навигационное свойство
        /// <summary>
        /// Коллекция сторон процесса
        /// </summary>
        [Display(Name = "Стороны процесса")]
        public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();

        public override string ToString() => "Справочник";
    }
}
