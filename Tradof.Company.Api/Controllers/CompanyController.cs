﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tradof.CompanyModule.Services.DTOs;
using Tradof.CompanyModule.Services.Interfaces;

namespace Tradof.Company.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string CompanyId, ChangeCompanyPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _companyService.ChangeCompanyPasswordAsync(CompanyId, dto);
                return Ok("Password changed successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
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
    }
}
