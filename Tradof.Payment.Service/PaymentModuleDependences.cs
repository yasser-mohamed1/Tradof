using Microsoft.Extensions.DependencyInjection;
using Tradof.Payment.Service.implemintation;
using Tradof.Payment.Service.Interfaces;


namespace Tradof.Payment.Service
{
    public static class PaymentModuleDependencies
    {
        public static IServiceCollection AddPaymentServices(this IServiceCollection service)
        {
            service.AddScoped<IPaymentService, PaymentService>();
            return service;
        }
    }
}