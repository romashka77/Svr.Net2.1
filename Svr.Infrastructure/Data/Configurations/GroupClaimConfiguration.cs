using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class GroupClaimConfiguration : IEntityTypeConfiguration<GroupClaim>
    {
        public void Configure(EntityTypeBuilder<GroupClaim> builder)
        {
            //builder.Property(d => d.Name).IsRequired().HasMaxLength(100).IsConcurrencyToken();
            builder.HasOne(d => d.CategoryDispute).WithMany(r => r.GroupClaims).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
