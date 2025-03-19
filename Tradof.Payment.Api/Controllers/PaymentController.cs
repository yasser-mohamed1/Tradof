using Microsoft.AspNetCore.Mvc;
using Tradof.Payment.Service.DTOs;
using Tradof.Payment.Service.Interfaces;

namespace Tradof.Payment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController(IPaymentService _paymentService) : ControllerBase
    {
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] InitiatePaymentRequest request)
        {
            var response = await _paymentService.InitiateSubscriptionPayment(request);
            return Ok(response);
        }

        [HttpPost("callback/paymob")]
        public async Task<IActionResult> HandlePaymobCallback([FromBody] PaymentCallbackRequest request)
        {
            await _paymentService.HandleCallback(request);
            return Ok();
        }
    }
}
