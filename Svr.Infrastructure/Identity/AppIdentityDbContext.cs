using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Svr.Infrastructure.Identity.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Identity
{
    //    По умолчанию данный класс наследует весь функционал от IdentityDbContext.Так, мы можем получить содержимое таблиц из бд с помощью следующих свойств:
    //Users: набор объектов IdentityUser, соответствует таблице пользователей
    //Roles: набор объектов IdentityRole, соответствует таблице ролей
    //RoleClaims: набор объектов IdentityRoleClaim, соответствует таблице связи ролей и объектов claims
    //UserLogins: набор объектов IdentityUserLogin, соответствует таблице связи пользователей с их логинами их внешних сервисов
    //UserClaims: набор объектов IdentityUserClaim, соответствует таблице связи пользователей и объектов claims
    //UserRoles: набор объектов IdentityUserRole, соответствует таблице, которая сопоставляет пользователей и их роли
    //UserTokens: набор объектов IdentityUserToken, соответствует таблице токенов пользователей
    public sealed class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly string schema = "identity";
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
            //автоматическая проверка наличия базы данных и, если она отсуствует, создаст ее.
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent API, я использую анатации https://metanit.com/sharp/entityframeworkcore/2.3.php
            modelBuilder.HasDefaultSchema(schema);
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
