using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public class RegionSpecification : BaseSpecification<Region>
    {
        public RegionSpecification(long? id) : base(i => (!id.HasValue || i.Id == id))
        {
        }
    }
}
