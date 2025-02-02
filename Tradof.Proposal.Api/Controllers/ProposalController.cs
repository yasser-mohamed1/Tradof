using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tradof.Data.SpecificationParams;
using Tradof.Proposal.Services.DTOs;
using Tradof.Proposal.Services.Interfaces;


namespace Tradof.Proposal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropsalController(IProposalService _proposalService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProposalSpecParams specParams)
        {
            return Ok(await _proposalService.GetAllAsync(specParams));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var project = await _proposalService.GetByIdAsync(id);
            return project != null ? Ok(project) : NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateProposalDto projectDto)
        {
            return Ok(await _proposalService.CreateAsync(projectDto));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProposalDto projectDto)
        {
            return Ok(await _proposalService.UpdateAsync(projectDto));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return Ok(await _proposalService.DeleteAsync(id));

        }
    }
}