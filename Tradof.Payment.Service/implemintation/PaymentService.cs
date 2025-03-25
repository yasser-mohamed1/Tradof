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
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("User not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id);

            if (company == null)
            {
                throw new Exception("Company not found");
            }

            // Check if the user already has an active subscription
            var activeSubscriptions = await _unitOfWork.Repository<CompanySubscription>()
                .FindAsync(s => s.CompanyId == company.Id && s.Status == SubscriptionStatus.Active);

            if (activeSubscriptions.Any())
            {
                throw new Exception("You are already subscribed to a plan");
            }

            try
            {
                var package = await _unitOfWork.Repository<Package>().GetByIdAsync(request.PackageId)
                    ?? throw new ArgumentException("Package not found");

                // Generate payment details
                var authToken = await _paymobClient.AuthenticateAsync();
                var amount = Convert.ToDecimal(package.Price);
                var order = await _paymobClient.CreateOrderAsync(authToken, amount);

                // Create a pending subscription record with the transaction reference
                var pendingSubscription = new PendingSubscription
                {
                    CompanyId = company.Id,
                    PackageId = package.Id,
                    Amount = package.Price,
                    TransactionReference = order.id.ToString(), // Set TransactionReference here
                    CreationDate = DateTime.UtcNow,
                    ModifiedBy = "system",
                    CreatedBy = "system"
                };

                await _unitOfWork.Repository<PendingSubscription>().AddAsync(pendingSubscription);
                await _unitOfWork.CommitAsync();

                // Generate payment key
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

        public async Task HandleCallback(PaymentCallbackRequest request, string hmac)
        {
            try
            {
                _logger.LogInformation("Processing payment callback");

                // Validate HMAC signature
                if (!ValidateHmac(request.Obj, hmac)) // Use the HMAC from the header
                {
                    _logger.LogWarning("Invalid HMAC signature");
                    throw new SecurityException("Invalid HMAC signature");
                }

                var response = request.Obj;

                if (!response.Success)
                {
                    _logger.LogWarning("Payment failed for transaction {TransactionId}", response.TransactionId);
                    return;
                }

                // Find the pending subscription using the transaction reference
                var pendingSubscription = await _unitOfWork.Repository<PendingSubscription>()
                    .FindFirstAsync(s => s.TransactionReference == response.Order.Id.ToString());  // Convert to string if necessary

                if (pendingSubscription == null)
                {
                    _logger.LogError("Pending subscription not found for order {OrderId}", response.Order.Id);
                    throw new ArgumentException("Pending subscription not found");
                }

                // Create the actual subscription
                var package = await _unitOfWork.Repository<Package>().GetByIdAsync(pendingSubscription.PackageId);
                if (package == null)
                {
                    _logger.LogError("Package not found for pending subscription {PendingSubscriptionId}", pendingSubscription.Id);
                    throw new ArgumentException("Package not found");
                }

                var subscription = new CompanySubscription
                {
                    CompanyId = pendingSubscription.CompanyId,
                    PackageId = package.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(package.DurationInMonths),
                    NetPrice = pendingSubscription.Amount,
                    Status = SubscriptionStatus.Active,
                    PaymentDate = DateTime.UtcNow,
                    CreationDate = DateTime.UtcNow,
                    ModificationDate = DateTime.UtcNow,
                    ModifiedBy = "system",
                    CreatedBy = "system"
                };

                await _unitOfWork.Repository<CompanySubscription>().AddAsync(subscription);
                await _unitOfWork.Repository<PendingSubscription>().DeleteAsync(pendingSubscription.Id);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Subscription {SubscriptionId} activated for company {CompanyId}", subscription.Id, subscription.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Callback processing failed");
                throw;
            }
        }

        private bool ValidateHmac(PaymobCallbackResponse obj, string receivedHmac)
        {
            try
            {
                // Serialize the obj to a JSON string
                var jsonString = JsonSerializer.Serialize(obj);

                // Compute the HMAC for the JSON string
                var computedHmac = ComputeHmac(jsonString, _config.HmacSecret);

                // Compare the computed HMAC with the received HMAC
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