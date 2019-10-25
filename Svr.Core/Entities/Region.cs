using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Регион (область)
    /// </summary>
    public class Region : BaseEntityCode
    {
        /// <summary>
        /// Коллекция районов
        /// </summary>
        [Display(Name = "Районы")]
        public virtual ICollection<District> Districts { get; set; } = new List<District>();

        /// <summary>
        /// Коллекция исполнителей
        /// </summary>
        [Display(Name = "Исполнители")]
        public virtual ICollection<Performer> Performers { get; set; } = new List<Performer>();

        public override string ToString() => "Регион";
    }
}
