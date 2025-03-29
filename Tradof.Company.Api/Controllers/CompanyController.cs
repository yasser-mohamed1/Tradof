using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tradof.CompanyModule.Services.DTOs;
using Tradof.CompanyModule.Services.Interfaces;

namespace Tradof.Company.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompanyController(ICompanyService _companyService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();

            return Ok(company);
        }

        [HttpGet("{id}/subscription")]
        public async Task<ActionResult<CompanySubscriptionDto>> GetCurrentSubscription(string Id)
        {
            var subscription = await _companyService.GetCurrentSubscriptionAsync(Id);

            if (subscription == null)
            {
                return NotFound("No current subscription found.");
            }

            return Ok(subscription);
        }


        [HttpGet("{id}/employees")]
        public async Task<IActionResult> GetCompanyEmployees(string id)
        {
            var employees = await _companyService.GetCompanyEmployeesAsync(id);
            if (employees == null || !employees.Any()) return NotFound("No employees found for this company.");

            return Ok(employees);
        }

        [HttpPost("{CompanyId}/ChangePassword")]
        public async Task<IActionResult> ChangePassword(string CompanyId, ChangeCompanyPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _companyService.ChangeCompanyPasswordAsync(CompanyId, dto);
            if (!result) return BadRequest("Failed to change password.");

            return NoContent();
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee(CreateCompanyEmployeeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _companyService.AddEmployeeAsync(dto);
            return Ok("Employee Added successfully");
        }

        [HttpPut("UpdateCompany")]
        public async Task<IActionResult> UpdateCompany(UpdateCompanyDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _companyService.UpdateCompanyAsync(dto);
            return Ok("Company updated successfully");
        }

        [HttpPost("AddLanguage/{companyId}")]
        public async Task<IActionResult> AddLanguage(string companyId, IEnumerable<long> languageIds)
        {
            await _companyService.AddLanguagesAsync(companyId, languageIds);
            return Ok("Language added successfully");
        }

        [HttpDelete("RemoveLanguage/{companyId}")]
        public async Task<IActionResult> RemoveLanguage(string companyId, IEnumerable<long> languageIds)
        {
            await _companyService.RemoveLanguagesAsync(companyId, languageIds);
            return Ok("Language removed successfully");
        }

        [HttpPost("AddSpecialization/{companyId}")]
        public async Task<IActionResult> AddSpecialization(string companyId, IEnumerable<long> specializationIds)
        {
            await _companyService.AddSpecializationsAsync(companyId, specializationIds);
            return Ok("Specialization added successfully");
        }

        [HttpDelete("RemoveSpecialization/{companyId}")]
        public async Task<IActionResult> RemoveSpecialization(string companyId, IEnumerable<long> specializationIds)
        {
            await _companyService.RemoveSpecializationsAsync(companyId, specializationIds);
            return Ok("Specialization removed successfully");
        }

        [HttpPost("{companyId}/social-medias/add-or-update-or-remove")]
        public async Task<IActionResult> AddOrUpdateOrRemoveSocialMedias(string companyId, [FromBody] IEnumerable<CreateSocialMediaDto> socialMedias)
        {
            if (socialMedias == null || !socialMedias.Any())
                return BadRequest("No social media data provided.");

            try
            {
                await _companyService.AddOrUpdateOrRemoveSocialMediasAsync(companyId, socialMedias);
                return Ok("Company social media list updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
