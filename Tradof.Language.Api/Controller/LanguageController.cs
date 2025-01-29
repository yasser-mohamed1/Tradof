using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Tradof.Language.Services.DTOs;
using Tradof.Language.Services.Interfaces;

namespace Tradof.Language.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController(ILanguageService _languageService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var languages = await _languageService.GetAllAsync();
            return Ok(languages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var language = await _languageService.GetByIdAsync(id);
                return Ok(language);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLanguageDto dto)
        {
            try
            {
                var language = await _languageService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = language.Id }, language);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UpdateLanguageDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            try
            {
                var language = await _languageService.UpdateAsync(dto);
                return Ok(language);
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
                await _languageService.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
