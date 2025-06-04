using Microsoft.Extensions.DependencyInjection;
using Tradof.Auth.Services.Implementation;
using Tradof.Auth.Services.Interfaces;

namespace Tradof.Auth.Services
{
    public static class AuthModuleDependencies
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddMemoryCache();
            return services;
        }
    }
}
