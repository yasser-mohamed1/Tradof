using Microsoft.Extensions.DependencyInjection;
using Tradof.Data.Interfaces;
using Tradof.Language.Services.Implementation;
using Tradof.Language.Services.Interfaces;
using Tradof.Repository.Repository;
using LanguageEntity = Tradof.Data.Entities.Language;

namespace Tradof.Language.Services
{
    public static class LanguageModuleDependencies
    {
        public static IServiceCollection AddLanguageServices(this IServiceCollection service)
        {
            service.AddScoped<IGeneralRepository<LanguageEntity>, GeneralRepository<LanguageEntity>>();
            service.AddScoped<ILanguageService, LanguageService>();
            return service;
        }
    }
}
