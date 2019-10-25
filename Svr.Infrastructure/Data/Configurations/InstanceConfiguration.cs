using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class InstanceConfiguration : IEntityTypeConfiguration<Instance>
    {
        public void Configure(EntityTypeBuilder<Instance> builder)
        {
            builder.HasOne(d => d.Claim).WithMany(r => r.Instances).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
