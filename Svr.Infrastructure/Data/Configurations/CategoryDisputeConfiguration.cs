using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class CategoryDisputeConfiguration : IEntityTypeConfiguration<CategoryDispute>
    {
        public void Configure(EntityTypeBuilder<CategoryDispute> builder)
        {
            //builder.Property(d => d.Name).IsRequired().HasMaxLength(100).IsConcurrencyToken();
        }
    }
}
