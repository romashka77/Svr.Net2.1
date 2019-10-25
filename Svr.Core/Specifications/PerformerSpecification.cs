using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class PerformerSpecification : BaseSpecification<Performer>
    {
        public PerformerSpecification(long? id) : base(i => (!id.HasValue || i.Region.Id == id))
        {
            AddInclude(d => d.Region);
            AddInclude(d => d.DistrictPerformers);
        }
    }
}
