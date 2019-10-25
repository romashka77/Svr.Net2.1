using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Svr.Infrastructure.Data;

namespace Svr.Infrastructure
{
    public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        private static string DataConnectionString => new DatabaseConfiguration().GetDataConnectionString();
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseNpgsql(DataConnectionString);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
