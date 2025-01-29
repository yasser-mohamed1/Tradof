using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tradof.SpecializationModule.Services.DTOs;
using Tradof.SpecializationModule.Services.Interfaces;

namespace Tradof.Specialization.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecializationController(ISpecializationService _specializationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specializations = await _specializationService.GetAllAsync();
            return Ok(specializations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var specialization = await _specializationService.GetByIdAsync(id);
                return Ok(specialization);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSpecializationDto dto)
        {
            try
            {
                var specialization = await _specializationService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = specialization.Id }, specialization);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UpdateSpecializationDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            try
            {
                var specialization = await _specializationService.UpdateAsync(dto);
                return Ok(specialization);
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
                await _specializationService.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
