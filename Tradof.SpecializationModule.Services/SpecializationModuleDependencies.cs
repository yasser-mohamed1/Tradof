using Microsoft.Extensions.DependencyInjection;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;
using Tradof.Repository.Repository;
using Tradof.SpecializationModule.Services.Implementation;
using Tradof.SpecializationModule.Services.Interfaces;

namespace Tradof.SpecializationModule.Services
{
    public static class SpecializationModuleDependencies
    {
        public static IServiceCollection AddSpecializationServices(this IServiceCollection service)
        {
            service.AddScoped<IGeneralRepository<Specialization>, GeneralRepository<Specialization>>();
            service.AddScoped<ISpecializationService, SpecializationService>();
            return service;
        }
    }
}
