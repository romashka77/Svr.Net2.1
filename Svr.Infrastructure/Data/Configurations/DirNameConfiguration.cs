using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svr.Core.Entities;

namespace Svr.Infrastructure.Data.Configurations
{
    public class DirNameConfiguration : IEntityTypeConfiguration<DirName>
    {
        public void Configure(EntityTypeBuilder<DirName> builder)
        {

        }
    }
}
