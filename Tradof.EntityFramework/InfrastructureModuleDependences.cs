using Microsoft.Extensions.DependencyInjection;
using Tradof.EntityFramework.Helpers;

namespace Tradof.EntityFramework
{
    public static class EmailModuleDependences
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection service)
        {
            service.AddScoped<IEmailService, EmailService>();

            return service;
        }
    }
}
