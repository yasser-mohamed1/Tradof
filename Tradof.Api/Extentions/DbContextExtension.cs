using Hangfire;
using Microsoft.EntityFrameworkCore;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Api.Extentions
{
    public static class DbContextExtension
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {


            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<TradofDbContext>(options =>
                options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(TradofDbContext).Assembly.FullName)));

            //  Hangfire to the Connection String
            services.AddHangfire(config =>
                config.UseSqlServerStorage(connectionString));

            services.AddHangfireServer();
        }
    }
}
