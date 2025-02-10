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
               var key = Encoding.UTF8.GetBytes(configuration["JWT:Secret"]);
               var signingKey = new SymmetricSecurityKey(key);

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   IssuerSigningKey = signingKey
               };
           });
        }
    }
}
