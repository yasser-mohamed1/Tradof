using Microsoft.Extensions.DependencyInjection;
using Tradof.Admin.Services.Helpers;
using Tradof.Admin.Services.Implementation;
using Tradof.Admin.Services.Interfaces;

namespace Tradof.Admin.Services
{
    public static class AdminServicesDependences
    {
        public static IServiceCollection AddAdminServices(this IServiceCollection service)
        {
            service.AddTransient<IAuthenticationService, AuthenticationService>();
            service.AddTransient<IHelperService, HelperService>();
            service.AddScoped<IAdminService, AdminService>();
            return service;
        }
    }
}