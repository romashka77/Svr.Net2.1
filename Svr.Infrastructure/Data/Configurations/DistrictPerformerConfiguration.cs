using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class DistrictPerformerConfiguration : IEntityTypeConfiguration<DistrictPerformer>
    {
        public void Configure(EntityTypeBuilder<DistrictPerformer> builder)
        {
            builder.HasKey(t => new { t.DistrictId, t.PerformerId });
            builder.HasOne(pt => pt.District).WithMany(p => p.DistrictPerformers).HasForeignKey(pt => pt.DistrictId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pt => pt.Performer).WithMany(t => t.DistrictPerformers).HasForeignKey(pt => pt.PerformerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
