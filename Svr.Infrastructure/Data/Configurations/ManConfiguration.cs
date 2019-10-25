using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class ManConfiguration : IEntityTypeConfiguration<Man>
    {
        public void Configure(EntityTypeBuilder<Man> builder)
        {
            //builder.ToTable("Men", schema: new SchemaConfiguration.P);
            //builder.Property(m => m.LastName).IsRequired().HasMaxLength(100).IsConcurrencyToken();
            //builder.Property(m => m.FirstName).IsRequired().HasMaxLength(100).IsConcurrencyToken();
            //builder.Property(m => m.MiddleName).HasMaxLength(100).IsConcurrencyToken();
            /*.HasAlternateKey(m => new { m.Snils, m.LastName, m.FirstName, m.MiddleName })*/

        }
    }
}
