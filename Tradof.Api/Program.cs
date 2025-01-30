using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Tradof.Admin.Services;
using Tradof.Auth.Services;
using Tradof.CountryModule.Services;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;
using Tradof.Language.Services;
using Tradof.PackageNamespace.Services;
using Tradof.Project.Services;
using Tradof.Repository;
using Tradof.SpecializationModule.Services;
using Tradof.CompanyModule.Services;

namespace Tradof.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;


            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            builder.Services.AddSwaggerGen();

            DotNetEnv.Env.Load();

            //#region Connection String
            //builder.Services.AddDbContext<TradofDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            //    b => b.MigrationsAssembly(typeof(TradofDbContext).Assembly.FullName)));
            //#endregion

            #region Connection String
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<TradofDbContext>(options =>
                options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(TradofDbContext).Assembly.FullName)));

            //  Hangfire to the Connection String
            builder.Services.AddHangfire(config =>
                config.UseSqlServerStorage(connectionString));

            builder.Services.AddHangfireServer();
            #endregion

            #region Identity
            builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<TradofDbContext>()
                    .AddDefaultTokenProviders();
            #endregion

            // #region Authentication

            // builder.Services.AddAuthentication(option =>
            // {
            //     option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //     option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            // })
            //.AddJwtBearer(options =>
            //{
            //    options.SaveToken = true;
            //    options.RequireHttpsMetadata = false;

            //    var validIssuers = configuration.GetSection("JWT:Issuer").Get<string[]>();
            //    var validAudiences = configuration.GetSection("JWT:Audience").Get<string[]>();

            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = false,
            //        //ValidIssuer = configuration["JWT:ValidIssur"],
            //        //ValidIssuers = validIssuers,
            //        ValidateAudience = false,
            //        //ValidAudiences = validAudiences,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
            //    };
            //});

            // #endregion

            #region Swagger 
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
            new List<string>()
        }
    });
            });
            #endregion

            #region Dependency Injection
            builder.Services.AddInfrastructureServices()
                            .AddReposetoriesServices()
                            .AddPackageServices()
                            .AddAuthServices()
                            .AddLanguageServices()
                            .AddCountryServices()
                            .AddSpecializationServices()
                            .AddProjectServices()
                            .AddCompanyServices();
            builder.Services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();

            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();

            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseMiddleware<PerformanceMiddleware>();

            app.MapControllers();

            app.MapGroup("api/auth").MapIdentityApi<ApplicationUser>();

            app.Run();
        }
    }
}
