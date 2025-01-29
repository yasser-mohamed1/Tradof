using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tradof.CountryModule.Services.DTOs;
using Tradof.CountryModule.Services.Interfaces;

namespace Tradof.Country.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController(ICountryService _countryService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _countryService.GetAllAsync();
            return Ok(countries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var country = await _countryService.GetByIdAsync(id);
                return Ok(country);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCountryDto dto)
        {
            try
            {
                var country = await _countryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = country.Id }, country);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UpdateCountryDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            try
            {
                var country = await _countryService.UpdateAsync(dto);
                return Ok(country);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _countryService.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
