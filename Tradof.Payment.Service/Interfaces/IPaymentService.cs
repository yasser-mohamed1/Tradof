using Tradof.Payment.Service.DTOs;

namespace Tradof.Payment.Service.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> InitiateSubscriptionPayment(InitiatePaymentRequest request);
        Task HandleCallback(PaymentCallbackRequest request);
    }
}