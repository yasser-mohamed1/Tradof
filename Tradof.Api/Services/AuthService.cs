using Grpc.Core;
using GrpcAuthService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tradof.Api.Services
{
    public class AuthService : GrpcAuthService.AuthService.AuthServiceBase
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override Task<TokenResponse> ValidateToken(TokenRequest request, ServerCallContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(request.Token, parameters, out SecurityToken validatedToken);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;

                return Task.FromResult(new TokenResponse
                {
                    IsValid = true,
                    UserId = userId ?? "",
                    Role = role ?? ""
                });
            }
            catch
            {
                return Task.FromResult(new TokenResponse { IsValid = false });
            }
        }
    }
}
