using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Исполнитель
    /// </summary>
    public class Performer : BaseEntityDescription
    {
        /// <summary>
        /// Id Региона (область)
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long RegionId { get; set; }
        /// <summary>
        /// Регион
        /// </summary>
        //[ForeignKey("RegionId")]
        [Display(Name = "Регион")]
        public virtual Region Region { get; set; }

        /// <summary>
        /// Коллекция районов
        /// </summary>
        [Display(Name = "Районы")]
        public virtual ICollection<DistrictPerformer> DistrictPerformers { get; set; } = new List<DistrictPerformer>();

        public override string ToString() => "Исполнитель";
    }
}
