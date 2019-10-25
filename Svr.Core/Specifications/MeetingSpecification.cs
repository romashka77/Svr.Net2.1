using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class MeetingSpecification : BaseSpecification<Meeting>
    {
        public MeetingSpecification(long? id) : base(i => (!id.HasValue || i.Claim.Id == id))
        {
            AddInclude(d => d.Claim);
        }
    }
}
