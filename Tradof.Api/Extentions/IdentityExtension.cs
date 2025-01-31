using Microsoft.AspNetCore.Identity;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Api.Extentions
{
    public static class IdentityExtension
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<ApplicationUser>().AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<TradofDbContext>()
                    .AddDefaultTokenProviders();
        }
    }
}
