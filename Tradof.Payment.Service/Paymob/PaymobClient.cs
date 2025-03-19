using System.Text;
using System.Text.Json;

namespace Tradof.Payment.Service.Paymob
{
    public class PaymobClient
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobConfig _config;

        public PaymobClient(HttpClient httpClient, PaymobConfig config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/api/");
        }

        public async Task<string> AuthenticateAsync()
        {
            var request = new { api_key = _config.ApiKey };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/tokens", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseContent);
            return doc.RootElement.GetProperty("token").GetString();
        }

        public async Task<OrderResponse> CreateOrderAsync(string authToken, decimal amount, string currency = "EGP")
        {
            var request = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (long)(amount * 100m),
                currency,
                items = new[] {
                    new {
                        name = "Subscription Package",
                        amount_cents = (long)(amount * 100m),
                        description = "Company subscription package",
                        quantity = "1"
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("ecommerce/orders", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Order creation failed: {errorContent}");
            }

            return JsonSerializer.Deserialize<OrderResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<PaymentKeyResponse> GeneratePaymentKeyAsync(
            string authToken,
            decimal amount,
            string orderId,
            Customer customer,
            string currency = "EGP")
        {
            var request = new
            {
                auth_token = authToken,
                amount_cents = (long)(amount * 100m),
                expiration = 3600,
                order_id = orderId,
                billing_data = customer,
                currency,
                integration_id = _config.IntegrationId
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("acceptance/payment_keys", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Payment key failed: {errorContent}");
            }

            return JsonSerializer.Deserialize<PaymentKeyResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public record OrderResponse(long id);
        public record PaymentKeyResponse(string token);
        public record Customer(
    string first_name,
    string last_name,
    string email,
    string phone_number,
    string street,
    string city,
    string country,
    string apartment = "N/A",      // Required by Paymob
    string floor = "N/A",          // Required by Paymob
    string building = "N/A",       // Required by Paymob
    string postal_code = "00000",  // Often required
    string state = "N/A"           // Sometimes required
);
    }
}