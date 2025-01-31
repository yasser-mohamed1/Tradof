using Microsoft.Extensions.DependencyInjection;
using Tradof.CompanyModule.Services.Implementation;
using Tradof.CompanyModule.Services.Interfaces;

namespace Tradof.CompanyModule.Services
{
    public static class CompanyModuleDependencies
    {
        public static IServiceCollection AddCompanyServices(this IServiceCollection service)
        {
            service.AddScoped<ICompanyService, CompanyService>();
            service.AddScoped<IEmailService, EmailService>();
            return service;
        }
    }
}