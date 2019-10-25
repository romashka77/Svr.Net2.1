using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class SubjectClaimSpecification : BaseSpecification<SubjectClaim>
    {
        public SubjectClaimSpecification(long? id) : base(i => (!id.HasValue || i.GroupClaim.Id == id))
        {
            AddInclude(d => d.GroupClaim);
        }
    }
}
