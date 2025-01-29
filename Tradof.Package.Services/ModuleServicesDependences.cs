using Microsoft.Extensions.DependencyInjection;
using Tradof.PackageNamespace.Services.Implementation;
using Tradof.PackageNamespace.Services.Interfaces;

namespace Tradof.PackageNamespace.Services
{
    public static class PackageModuleDependencies
    {
        public static IServiceCollection AddPackageServices(this IServiceCollection service)
        {
            service.AddScoped<IPackageService, PackageService>();
            return service;
        }
    }
}