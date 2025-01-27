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
            services.AddScoped<IFreelancerRepository, FreelancerRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IFreelancerLanguagesPairRepository, FreelancerLanguagesPairRepository>();
            services.AddMemoryCache();

            return services;
        }
    }
}
