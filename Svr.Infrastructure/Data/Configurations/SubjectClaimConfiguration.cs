using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class SubjectClaimConfiguration : IEntityTypeConfiguration<SubjectClaim>
    {
        public void Configure(EntityTypeBuilder<SubjectClaim> builder)
        {
            //builder.Property(d => d.Name).IsRequired().HasMaxLength(100).IsConcurrencyToken();
            builder.HasOne(d => d.GroupClaim).WithMany(r => r.SubjectClaims).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
