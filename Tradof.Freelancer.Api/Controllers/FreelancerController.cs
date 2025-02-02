using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tradof.FreelancerModule.Services.DTOs;
using Tradof.FreelancerModule.Services.Interfaces;

namespace Tradof.Freelancer.Api.Controllers;

[ApiController]
[Route("api/freelancers")]
public class FreelancerController(IFreelancerService _freelancerService) : ControllerBase
{
    [HttpGet("{freelancerId}")]
    public async Task<IActionResult> GetFreelancerData(string freelancerId)
    {
        var freelancerData = await _freelancerService.GetFreelancerDataAsync(freelancerId);
        if (freelancerData == null) return NotFound("Freelancer not found.");
        return Ok(freelancerData);
    }

    [HttpPut("{freelancerId}")]
    public async Task<IActionResult> UpdateFreelancer(string freelancerId, [FromBody] UpdateFreelancerDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _freelancerService.UpdateFreelancerAsync(freelancerId, dto);
        if (!result) return NotFound("Freelancer not found.");

        return NoContent();
    }

    [HttpPost("{freelancerId}/upload-cv")]
    public async Task<IActionResult> UploadCV(string freelancerId, IFormFile file)
    {
        if (file == null) return BadRequest("CV file is required.");
        var result = await _freelancerService.UploadCVAsync(freelancerId, file);
        if (result == null) return BadRequest("Failed to upload CV.");
        return Ok(new { CvUrl = result });
    }

    [HttpPost("{freelancerId}/social-medias")]
    public async Task<IActionResult> AddFreelancerSocialMedia(string freelancerId, [FromBody] AddFreelancerSocialMediaDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _freelancerService.AddFreelancerSocialMediaAsync(freelancerId, dto);
        if (!result) return BadRequest("Failed to add social media.");

        return CreatedAtAction(nameof(GetFreelancerData), new { freelancerId }, dto);
    }

    [HttpPut("social-medias/{socialMediaId:long}")]
    public async Task<IActionResult> UpdateFreelancerSocialMedia(long socialMediaId, [FromBody] UpdateFreelancerSocialMediaDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _freelancerService.UpdateFreelancerSocialMediaAsync(socialMediaId, dto);
        if (!result) return NotFound("Social media not found.");

        return NoContent();
    }

    [HttpPost("{freelancerId}/language-pairs")]
    public async Task<IActionResult> AddFreelancerLanguagePair(string freelancerId, [FromBody] AddFreelancerLanguagePairDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _freelancerService.AddFreelancerLanguagePairAsync(freelancerId, dto);
        if (!result) return BadRequest("Failed to add the language pair or it already exists.");

        return CreatedAtAction(nameof(AddFreelancerLanguagePair), new { freelancerId }, dto);
    }

    [HttpDelete("language-pairs/{languagePairId:long}")]
    public async Task<IActionResult> RemoveFreelancerLanguagePair(long languagePairId)
    {
        var result = await _freelancerService.RemoveFreelancerLanguagePairAsync(languagePairId);
        if (!result) return NotFound("Language pair not found.");

        return NoContent();
    }

    [HttpPost("{freelancerId}/change-password")]
    public async Task<IActionResult> ChangePassword(string freelancerId, [FromBody] ChangePasswordDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _freelancerService.ChangePasswordAsync(freelancerId, dto);
        if (!result) return BadRequest("Failed to change password.");

        return NoContent();
    }

    [HttpPost("{freelancerId}/payment-methods")]
    public async Task<IActionResult> AddPaymentMethod(string freelancerId, [FromBody] PaymentMethodDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _freelancerService.AddPaymentMethodAsync(freelancerId, dto);
        if (!result) return BadRequest("Failed to add payment method.");

        return CreatedAtAction(nameof(GetFreelancerData), new { freelancerId }, dto);
    }

    [HttpDelete("payment-methods/{paymentMethodId:long}")]
    public async Task<IActionResult> RemovePaymentMethod(long paymentMethodId)
    {
        var result = await _freelancerService.RemovePaymentMethodAsync(paymentMethodId);
        if (!result) return NotFound("Payment method not found.");

        return NoContent();
    }
}
