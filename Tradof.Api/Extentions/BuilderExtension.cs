using Hangfire;
using Tradof.Data.Entities;

namespace Tradof.Api.Extentions
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

            app.UseStaticFiles();

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
