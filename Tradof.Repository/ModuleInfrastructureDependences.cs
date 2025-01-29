using Microsoft.Extensions.DependencyInjection;
using Tradof.Data.Interfaces;
using Tradof.Repository.Repository;

namespace Tradof.Repository
{
    public static class ModuleInfrastructureDependences
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection service)
        {
            service.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            return service;
        }
    }
}
