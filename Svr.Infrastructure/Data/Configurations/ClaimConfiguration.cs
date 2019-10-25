using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.HasOne(d => d.District).WithMany(r => r.Claims).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
