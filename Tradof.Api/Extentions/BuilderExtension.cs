using Hangfire;
using Tradof.Api;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Extentions
{
    public static class BuilderExtension
    {
        public static void UseSwaggerConfiguration(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        public static void UseCustomMiddlewares(this WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseMiddleware<PerformanceMiddleware>();

            app.MapControllers();

            app.MapGroup("api/auth").MapIdentityApi<ApplicationUser>();
        }
    }
}
