using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class DistrictSpecification : BaseSpecification<District>
    {
        public DistrictSpecification(long? id) : base(i => (!id.HasValue || i.Region.Id == id))
        {
            AddInclude(d => d.Region);
            AddInclude(d => d.DistrictPerformers);
            //AddInclude(i => i.Region == region);
            //AddInclude($"{nameof(Region.Districts)}.{nameof(District.Region)}");

        }
    }
}
