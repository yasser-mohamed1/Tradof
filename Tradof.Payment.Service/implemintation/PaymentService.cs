using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;
using Tradof.EntityFramework.Helpers;
using Tradof.Payment.Service.DTOs;
using Tradof.Payment.Service.Interfaces;
using Tradof.Payment.Service.Paymob;

namespace Tradof.Payment.Service.implemintation
{
    public class PaymentService(
        PaymobClient paymobClient,
        IOptions<PaymobConfig> config,
        IUnitOfWork unitOfWork,
        ILogger<PaymentService> logger,
         IUserHelpers userHelpers) : IPaymentService
    {
        private readonly PaymobClient _paymobClient = paymobClient;
        private readonly PaymobConfig _config = config.Value;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<PaymentService> _logger = logger;
        private readonly IUserHelpers _userHelpers = userHelpers;

        public async Task<PaymentResponse> InitiateSubscriptionPayment(InitiatePaymentRequest request)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id);
            var subscriptions = await _unitOfWork.Repository<CompanySubscription>().FindAsync(f => f.CompanyId == company.Id);


            if (company == null)
            {
                throw new Exception("company not found");
            }
            if (subscriptions != null)
            {
                foreach (var subscription in subscriptions)
                {
                    if (DateTime.UtcNow < subscription.EndDate)
                        throw new Exception("you already subscriped to a plan");
                }
            }
            try
            {
                var package = await _unitOfWork.Repository<Package>().GetByIdAsync(request.PackageId)
                    ?? throw new ArgumentException("Package not found");

                var subscription = new CompanySubscription
                {
                    CompanyId = company.Id,
                    PackageId = package.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(package.DurationInMonths),
                    NetPrice = package.Price,
                    Status = SubscriptionStatus.Pending,
                    CreationDate = DateTime.UtcNow,
                    ModificationDate = DateTime.UtcNow,
                    ModifiedBy = "system",
                    CreatedBy = "system"
                };

                await _unitOfWork.Repository<CompanySubscription>().AddAsync(subscription);
                await _unitOfWork.CommitAsync();

                var authToken = await _paymobClient.AuthenticateAsync();
                var amount = Convert.ToDecimal(package.Price);
                var order = await _paymobClient.CreateOrderAsync(authToken, amount);

                subscription.TransactionReference = order.id.ToString();
                await _unitOfWork.Repository<CompanySubscription>().UpdateAsync(subscription);
                await _unitOfWork.CommitAsync();

                var customer = new PaymobClient.Customer(
                    first_name: currentUser.FirstName,
                    last_name: currentUser.LastName,
                    email: currentUser.Email,
                    phone_number: FormatPhoneNumber(currentUser.PhoneNumber),
                    street: "123 Main St",
                    city: "Cairo",
                    country: "EGY",
                    apartment: "N/A",
                    floor: "0",
                    building: "N/A",
                    postal_code: "12345",
                    state: "Cairo"
                );

                var paymentKey = await _paymobClient.GeneratePaymentKeyAsync(
                    authToken,
                    amount,
                    order.id.ToString(),
                    customer
                );

                return new PaymentResponse
                {
                    PaymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_config.IframeId}?payment_token={paymentKey.token}",
                    TransactionReference = order.id.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment initiation failed");
                throw;
            }
        }

        private static string FormatPhoneNumber(string phone)
        {
            // Ensure Egyptian phone number format
            if (phone.StartsWith("+20")) return phone;
            if (phone.StartsWith("0")) return $"+20{phone[1..]}";
            return $"+20{phone}";
        }

        public async Task HandleCallback(PaymentCallbackRequest request)
        {
            try
            {
                _logger.LogInformation("Processing payment callback");

                if (!ValidateHmac(request.Obj, request.Hmac))
                {
                    _logger.LogWarning("Invalid HMAC signature");
                    throw new SecurityException("Invalid HMAC signature");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = JsonSerializer.Deserialize<PaymobCallbackResponse>(request.Obj, options);

                if (!response.Success)
                {
                    _logger.LogWarning("Payment failed for transaction {TransactionId}", response.TransactionId);
                    return;
                }

                var subscription = await _unitOfWork.Repository<CompanySubscription>()
                    .FindFirstAsync(s => s.TransactionReference == response.Order.Id);

                if (subscription == null)
                {
                    _logger.LogError("Subscription not found for order {OrderId}", response.Order.Id);
                    throw new ArgumentException("Subscription not found");
                }

                if (subscription.Status == SubscriptionStatus.Active)
                {
                    _logger.LogWarning("Subscription {SubscriptionId} already active", subscription.Id);
                    return;
                }

                subscription.Status = SubscriptionStatus.Active;
                subscription.PaymentDate = DateTime.UtcNow;
                subscription.NetPrice = (double)(response.AmountCents / 100);

                await _unitOfWork.Repository<CompanySubscription>().UpdateAsync(subscription);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Subscription {SubscriptionId} activated", subscription.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Callback processing failed");
                throw;
            }
        }

        private bool ValidateHmac(string obj, string receivedHmac)
        {
            try
            {
                var computedHmac = ComputeHmac(obj, _config.HmacSecret);
                return computedHmac.Equals(receivedHmac, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string ComputeHmac(string data, string secret)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}