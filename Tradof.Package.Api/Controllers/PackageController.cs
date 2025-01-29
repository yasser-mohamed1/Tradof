using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tradof.PackageNamespace.Services.DTOs;
using Tradof.PackageNamespace.Services.Interfaces;

namespace Tradof.Package.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController(IPackageService _packageService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _packageService.GetAllAsync();
            return Ok(packages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var package = await _packageService.GetByIdAsync(id);
                return Ok(package);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePackageDto dto)
        {
            try
            {
                var package = await _packageService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = package.Id }, package);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UpdatePackageDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            try
            {
                var package = await _packageService.UpdateAsync(dto);
                return Ok(package);
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
                await _packageService.DeleteAsync(id);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
