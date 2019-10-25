using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class ClaimSpecification : BaseSpecification<Claim>
    {
        public ClaimSpecification(long? id) : base(i => (!id.HasValue || i.District.Id == id))
        {
            AddInclude(d => d.District);
            AddInclude(d => d.CategoryDispute);
            AddInclude(d => d.GroupClaim);
            AddInclude(d => d.SubjectClaim);

            //AddInclude(d => d.Instances);

            //var dataContext = _context.Claims.Include(c => c.CategoryDispute).Include(c => c.District).Include(c => c.GroupClaim).Include(c => c.Performer).Include(c => c.Person3rd).Include(c => c.Plaintiff).Include(c => c.Region).Include(c => c.Respondent).Include(c => c.Сourt);
        }
    }
    public sealed class ClaimSpecificationReport : BaseSpecification<Claim>
    {
        public ClaimSpecificationReport(long? id) : base(i => (!id.HasValue || i.District.Id == id))
        {
            AddInclude(d => d.District);
            AddInclude(d => d.Instances);
            //var dataContext = _context.Claims.Include(c => c.CategoryDispute).Include(c => c.District).Include(c => c.GroupClaim).Include(c => c.Performer).Include(c => c.Person3rd).Include(c => c.Plaintiff).Include(c => c.Region).Include(c => c.Respondent).Include(c => c.Сourt);
        }
    }
}
