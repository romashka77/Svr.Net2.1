using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class InstanceSpecification : BaseSpecification<Instance>
    {
        public InstanceSpecification(long? id) : base(i => (!id.HasValue || i.Claim.Id == id))
        {
            AddInclude(d => d.Claim);
            //AddInclude(d => d.CourtDecision);
        }
    }
}
