using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tradof.Common.Enums;
using Tradof.Data.SpecificationParams;
using Tradof.Proposal.Services.DTOs;
using Tradof.Proposal.Services.Interfaces;


namespace Tradof.Proposal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProposalController(IProposalService _proposalService) : ControllerBase
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
        [Authorize]
        [HttpGet("countByMonth")]
        public async Task<IActionResult> GetProposalsCountByMonth(int year, int month, ProposalStatus status)
        {
            return Ok(await _proposalService.GetProposalsCountByMonth(year, month, status));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateProposalDto projectDto)
        {
            try
            {
                return Ok(await _proposalService.CreateAsync(projectDto));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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

        [HttpPost("accept")]
        public async Task<IActionResult> Accept(long projectId, long ProposalId)
        {
            return Ok(await _proposalService.AcceptProposal(projectId, ProposalId));
        }

        [HttpPost("deny")]
        public async Task<IActionResult> Deny(long projectId, long ProposalId)
        {
            return Ok(await _proposalService.DenyProposal(projectId, ProposalId));

        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(long ProposalId)
        {
            return Ok(await _proposalService.CancelProposal(ProposalId));

        }
    }
}