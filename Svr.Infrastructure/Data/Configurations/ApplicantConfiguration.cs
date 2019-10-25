using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
    {
        public void Configure(EntityTypeBuilder<Applicant> builder)
        {
            builder.HasOne(d => d.TypeApplicant).WithMany(r => r.Applicants).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
