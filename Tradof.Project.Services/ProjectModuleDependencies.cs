using Microsoft.Extensions.DependencyInjection;
using Tradof.Project.Services.BackgroundServices;
using Tradof.Project.Services.Implementation;
using Tradof.Project.Services.Interfaces;

namespace Tradof.Project.Services
{
    public static class ProjectModuleDependencies
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectNotificationService, ProjectNotificationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddHostedService<ProjectNotificationBackgroundService>();
            services.AddHttpClient();
            return services;
        }
    }
}