using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Svr.Infrastructure.Data;
using Svr.Infrastructure.Identity;

namespace Svr.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Создание менеджера пользователей
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    // Создание менеджера ролей
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    /*await */AppIdentityDbContextSeed.SeedAsync(userManager, rolesManager);

                    var dataContext = services.GetRequiredService<DataContext>();
                    //static
                    DataContextSeed.SeedAsync(dataContext/*, loggerFactory*/).Wait();

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Произошла ошибка при заполнении базы данных.");
                }
            }
            host.Run();


            //CreateWebHostBuilder(args).Build().Run();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
    }
}
