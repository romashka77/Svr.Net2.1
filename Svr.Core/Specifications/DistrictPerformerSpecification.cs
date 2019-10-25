using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public class DistrictPerformerSpecification : BaseSpecification<DistrictPerformer>
    {
        public DistrictPerformerSpecification(long? id) : base(i => (!id.HasValue || i.DistrictId == id))
        {
        }

    }
}
