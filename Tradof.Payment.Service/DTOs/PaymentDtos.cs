using System.Text.Json.Serialization;

namespace Tradof.Payment.Service.DTOs
{
    public class InitiatePaymentRequest
    {
        public long PackageId { get; set; }
    }

    public class PaymentResponse
    {
        public string PaymentUrl { get; set; }
        public string TransactionReference { get; set; }
    }

    public class PaymentCallbackRequest
    {
        public PaymobCallbackResponse Obj { get; set; }
    }

    public class PaymobCallbackResponse
    {
        public bool Success { get; set; }
        public PaymobOrder Order { get; set; }
        public long AmountCents { get; set; }
        public string TransactionId { get; set; }
    }

    public class PaymobOrder
    {
        [JsonPropertyName("id")]
        public long Id { get; set; } // Use long for numeric IDs
    }
}