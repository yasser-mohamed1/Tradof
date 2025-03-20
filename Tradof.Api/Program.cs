using Hangfire;
using Tradof.Admin.Services;
using Tradof.Api.Extentions;
using Tradof.Auth.Services;
using Tradof.CompanyModule.Services;
using Tradof.CountryModule.Services;
using Tradof.FreelancerModule.Services;
using Tradof.Language.Services;
using Tradof.PackageNamespace.Services;
using Tradof.Payment.Service.implemintation;
using Tradof.Payment.Service.Interfaces;
using Tradof.Payment.Service.Paymob;
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

// Register PaymobConfig from appsettings.json
builder.Services.Configure<PaymobConfig>(builder.Configuration.GetSection("Paymob"));

// Explicitly register PaymobConfig as a service
builder.Services.AddSingleton(provider =>
{
    var config = new PaymobConfig();
    builder.Configuration.GetSection("Paymob").Bind(config);
    return config;
});

// Register PaymobClient with HttpClient
builder.Services.AddHttpClient<PaymobClient>((provider, client) =>
{
    var config = provider.GetRequiredService<PaymobConfig>();
    client.BaseAddress = new Uri("https://accept.paymob.com/api/");
});

// Register other services
builder.Services.AddInfrastructureServices().AddReposetoriesServices();
builder.Services.AddPackageServices()
                .AddAuthServices()
                .AddLanguageServices()
                .AddCountryServices()
                .AddSpecializationServices()
                .AddProjectServices()
                .AddCompanyServices()
                .AddFreelancerServices()
                .AddProposalServices();

// Register PaymentService
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Register Hangfire
builder.Services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwaggerConfiguration();

app.UseCustomMiddlewares();

app.Run();