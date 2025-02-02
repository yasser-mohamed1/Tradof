using Microsoft.Extensions.DependencyInjection;
using Tradof.FreelancerModule.Services.Implementation;
using Tradof.FreelancerModule.Services.Interfaces;

namespace Tradof.FreelancerModule.Services
{
    public static class FreelancerModuleDependencies
    {
        public static IServiceCollection AddFreelancerServices(this IServiceCollection service)
        {
            service.AddScoped<IFreelancerService, FreelancerService>();
            return service;
        }
    }
}