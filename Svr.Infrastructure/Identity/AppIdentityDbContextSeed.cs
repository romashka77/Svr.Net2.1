using Microsoft.AspNetCore.Identity;
using Svr.Core.Entities;
using Svr.Core.Extensions;
using System;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Identity
{
    /// <summary>
    /// Класс для создания администратора и ролей
    /// </summary>
    public class AppIdentityDbContextSeed

    {
        /// <summary>
        /// Создание администратора и ролей
        /// </summary>
        /// <param name="userManager">Менеджер пользователей</param>
        /// <param name="roleManager">Менеджер ролей</param>
        /// <returns></returns>
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string adminEmail = "romashka_77@mail.ru";
            const string firstName = "Роман";
            const string lastName = "Макаров";
            const string middleName = "Александрович";
            const string dateofBirth = "12.04.1977";
            const string password = "Ram270984";


            foreach (RoleState item in Enum.GetValues(typeof(RoleState)))
            {
                if (await roleManager.FindByNameAsync(item.GetDescription()) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(item.GetDescription()));
                }
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                ApplicationUser admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail, FirstName = firstName, LastName = lastName, MiddleName = middleName, DateofBirth = DateTime.Parse(dateofBirth) };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, RoleState.Administrator.GetDescription());
                }
            }
        }
    }
}
