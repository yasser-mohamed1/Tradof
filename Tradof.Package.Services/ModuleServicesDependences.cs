using Microsoft.Extensions.DependencyInjection;
using Tradof.Data.Interfaces;
using Tradof.Package.Services.Implementation;
using Tradof.Package.Services.Interfaces;
using Tradof.Repository.Repository;
using PackageEntity = Tradof.Data.Entities.Package;

namespace Tradof.Package.Services
{
    public static class PackageModuleDependencies
    {
        public static IServiceCollection AddPackageServices(this IServiceCollection service)
        {
            service.AddScoped<IGeneralRepository<PackageEntity>, GeneralRepository<PackageEntity>>();
            service.AddScoped<IPackageService, PackageService>();
            return service;
        }
    }
}