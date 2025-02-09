using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tradof.FreelancerModule.Services.DTOs;
using Tradof.FreelancerModule.Services.Interfaces;

namespace Tradof.Freelancer.Api.Controllers;

[ApiController]
[Route("api/freelancers")]
[Authorize(Roles = RoleNames.Freelancer)]
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

    [HttpPost("{freelancerId}/social-medias/add-or-update-or-remove")]
    public async Task<IActionResult> AddOrUpdateOrRemoveFreelancerSocialMediasAsync(string freelancerId, [FromBody] IEnumerable<AddFreelancerSocialMediaDTO> socialMedias)
    {
        if (socialMedias == null || !socialMedias.Any())
        {
            return BadRequest("Social media list cannot be empty.");
        }

        try
        {
            await _freelancerService.AddOrUpdateOrRemoveFreelancerSocialMediasAsync(freelancerId, socialMedias);
            return Ok("Freelancer social media list updated successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("{freelancerId}/language-pairs")]
    public async Task<IActionResult> AddFreelancerLanguagePair(string freelancerId, [FromBody] IEnumerable<AddFreelancerLanguagePairDTO> dtos)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _freelancerService.AddFreelancerLanguagePairsAsync(freelancerId, dtos);
        if (!result) return BadRequest("Failed to add the language pair or it already exists.");

        return CreatedAtAction(nameof(AddFreelancerLanguagePair), new { freelancerId }, dtos);
    }

    [HttpDelete("{freelancerId}/language-pairs")]
    public async Task<IActionResult> RemoveFreelancerLanguagePair(string freelancerId, IEnumerable<long> languagePairIds)
    {
        var result = await _freelancerService.RemoveFreelancerLanguagePairsAsync(freelancerId,languagePairIds);
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
