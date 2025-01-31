using Hangfire;
using Tradof.Admin.Services;
using Tradof.Api.Extentions;
using Tradof.Auth.Services;
using Tradof.CountryModule.Services;
using Tradof.EntityFramework.Extentions;
using Tradof.Language.Services;
using Tradof.PackageNamespace.Services;
using Tradof.Project.Services;
using Tradof.Proposal.Services;
using Tradof.Repository;
using Tradof.SpecializationModule.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.ConfigureCors();

DotNetEnv.Env.Load();

builder.Services.ConfigureDbContext(builder.Configuration);

builder.Services.ConfigureIdentity();

//builder.Services.ConfigureAuthentication(builder.Configuration);

builder.Services.ConfigureSwagger();

builder.Services.AddInfrastructureServices().AddReposetoriesServices();
builder.Services.AddPackageServices()
                .AddAuthServices()
                .AddLanguageServices()
                .AddCountryServices()
                .AddSpecializationServices()
                .AddProjectServices()
                .AddProposalServices();
builder.Services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwaggerConfiguration();

app.UseCustomMiddlewares();

app.Run();
