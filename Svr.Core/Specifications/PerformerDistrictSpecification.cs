using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public class PerformerDistrictSpecification : BaseSpecification<DistrictPerformer>
    {
        public PerformerDistrictSpecification(long? id) : base(i => (!id.HasValue || i.Performer.Id == id))
        {
        }
    }
}
