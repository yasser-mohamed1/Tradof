using Microsoft.Extensions.DependencyInjection;
using Tradof.Project.Services.Implementation;
using Tradof.Project.Services.Interfaces;


namespace Tradof.Project.Services
{
    public static class PackageModuleDependencies
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection service)
        {
            service.AddScoped<IProjectService, ProjectService>();

            return service;
        }
    }
}