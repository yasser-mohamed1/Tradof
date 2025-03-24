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

    public record PaymobCallbackResponse(
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("order")] PaymobOrder Order,
        [property: JsonPropertyName("amount_cents")] long AmountCents,
        [property: JsonPropertyName("id")] string TransactionId
    );

    public record PaymobOrder(
        [property: JsonPropertyName("id")] string Id
    );

    public class PaymentCallbackRequest
    {
        public string Hmac { get; set; }
        public string Obj { get; set; }  // This should contain the JSON payload from Paymob
    }
}