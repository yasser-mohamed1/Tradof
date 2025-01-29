using Microsoft.Extensions.DependencyInjection;
using Tradof.CountryModule.Services.Implementation;
using Tradof.CountryModule.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;
using Tradof.Repository.Repository;

namespace Tradof.CountryModule.Services
{
    public static class CountryModuleDependencies
    {
        public static IServiceCollection AddCountryServices(this IServiceCollection service)
        {
            service.AddScoped<IGeneralRepository<Country>, GeneralRepository<Country>>();
            service.AddScoped<ICountryService, CountryService>();
            return service;
        }
    }
}
