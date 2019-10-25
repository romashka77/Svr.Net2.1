using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class ApplicantSpecification : BaseSpecification<Applicant>
    {

        public ApplicantSpecification(long? id) : base(i => (!id.HasValue || i.TypeApplicant.Id == id))
        {
            AddInclude(d => d.TypeApplicant);
        }
    }
}
