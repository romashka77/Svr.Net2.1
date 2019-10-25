using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class GroupClaimSpecification : BaseSpecification<GroupClaim>
    {
        public GroupClaimSpecification(long? id) : base(i => (!id.HasValue || i.CategoryDispute.Id == id))
        {
            AddInclude(d => d.CategoryDispute);
        }
    }
    public sealed class GroupClaimSpecificationReport : BaseSpecification<GroupClaim>
    {
        public GroupClaimSpecificationReport(long? id) : base(i => (!id.HasValue || i.CategoryDispute.Id == id))
        {
            AddInclude(d => d.CategoryDispute);
            AddInclude(d => d.SubjectClaims);
        }
    }
}
