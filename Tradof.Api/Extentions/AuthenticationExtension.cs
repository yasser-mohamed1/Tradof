using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Tradof.Api.Extentions
{
    public static class AuthenticationExtension
    {
        public static void ConfigureAuthentication(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.RequireHttpsMetadata = false;

               var validIssuers = configuration.GetSection("JWT:Issuer").Get<string[]>();
               var validAudiences = configuration.GetSection("JWT:Audience").Get<string[]>();

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   //ValidIssuer = configuration["JWT:ValidIssur"],
                   //ValidIssuers = validIssuers,
                   ValidateAudience = false,
                   //ValidAudiences = validAudiences,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
               };
           });
        }
    }
}
