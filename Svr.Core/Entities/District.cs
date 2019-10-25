using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Район
    /// </summary>
    public class District : BaseEntityCode
    {
        /// <summary>
        /// Регион (область)
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long RegionId { get; set; }
        //[ForeignKey("RegionId")]
        [Display(Name = "Регион")]
        public virtual Region Region { get; set; }

        /// <summary>
        /// Коллекция исполнителей
        /// </summary>
        [Display(Name = "Исполнители")]
        public virtual ICollection<DistrictPerformer> DistrictPerformers { get; set; } = new List<DistrictPerformer>();

        /// <summary>
        /// Коллекция исков
        /// </summary>
        [Display(Name = "Иски")]
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

        public override string ToString() => "Район";
    }
}
