using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Infrastructure.Data.Configurations;
using Svr.Infrastructure.Data.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public sealed class DataContext : DbContext
    {
        private const string Schema = "jurist";

        public DbSet<Region> Regions { get; set; }
        public DbSet<District> Districts { get; set; }

        public DbSet<CategoryDispute> CategoryDisputes { get; set; }
        public DbSet<GroupClaim> GroupClaims { get; set; }
        public DbSet<SubjectClaim> SubjectClaims { get; set; }

        public DbSet<DirName> DirName { get; set; }
        public DbSet<Dir> Dir { get; set; }
        public DbSet<Applicant> Applicant { get; set; }

        public DbSet<Performer> Performers { get; set; }
        public DbSet<DistrictPerformer> DistrictPerformers { get; set; }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Instance> Instances { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<FileEntity> FileEntities { get; set; }



        //public DbSet<Man> Men { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //автоматическая проверка наличия базы данных и, если она отсуствует, создаст ее.
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent API, https://metanit.com/sharp/entityframeworkcore/2.3.php
            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.ApplyConfiguration(new RegionConfiguration());
            modelBuilder.ApplyConfiguration(new DistrictPerformerConfiguration());

            modelBuilder.ApplyConfiguration(new DistrictConfiguration());
            modelBuilder.ApplyConfiguration(new DistrictPerformerConfiguration());
            modelBuilder.ApplyConfiguration(new PerformerConfiguration());

            modelBuilder.ApplyConfiguration(new CategoryDisputeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupClaimConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectClaimConfiguration());

            modelBuilder.ApplyConfiguration(new DirNameConfiguration());
            modelBuilder.ApplyConfiguration(new DirConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicantConfiguration());

            modelBuilder.ApplyConfiguration(new ClaimConfiguration());
            modelBuilder.ApplyConfiguration(new InstanceConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingConfiguration());
            modelBuilder.ApplyConfiguration(new FileEntityConfiguration());

            //modelBuilder.ApplyConfiguration(new ManConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.ApplyAuditInformation();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangeOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.ApplyAuditInformation();
            return await base.SaveChangesAsync(acceptAllChangeOnSuccess, cancellationToken);
        }
    }
}
