﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tradof.Common.Enums;
using Tradof.Data.SpecificationParams;
using Tradof.Proposal.Services.DTOs;
using Tradof.Proposal.Services.Interfaces;
using Tradof.ResponseHandler.Consts;
using Tradof.ResponseHandler.Models;


namespace Tradof.Proposal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProposalController(IProposalService _proposalService) : ApiControllerBase
    {
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
        [HttpGet("countByMonth")]
        public async Task<IActionResult> GetProposalsCountByMonth(int year, int month, ProposalStatus status)
        {
            return Ok(await _proposalService.GetProposalsCountByMonth(year, month, status));
        }
        [HttpPost]
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
            try
            {
                var result = await _proposalService.DeleteAsync(id);

                if (!result)
                {
                    var failedResponse = APIOperationResponse<bool>.Fail(
                        ResponseType.BadRequest,
                        CommonErrorCodes.OperationFailed,
                        "Proposal could not be deleted."
                    );
                    return StatusCode(failedResponse.StatusCode, failedResponse);
                }

                var successResponse = APIOperationResponse<bool>.Success(true, "Proposal deleted successfully.");
                return StatusCode(successResponse.StatusCode, successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = APIOperationResponse<bool>.Fail(
                    ResponseType.InternalServerError,
                    CommonErrorCodes.ServerError,
                    ex.Message
                );
                return StatusCode(errorResponse.StatusCode, errorResponse);
            }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Accept(long projectId, long proposalId)
        {
            try
            {
                var result = await _proposalService.AcceptProposal(projectId, proposalId);

                if (!result)
                {
                    var failedResponse = APIOperationResponse<bool>.Fail(
                        ResponseType.BadRequest,
                        CommonErrorCodes.OperationFailed,
                        "Proposal could not be accepted."
                    );
                    return StatusCode(failedResponse.StatusCode, failedResponse);
                }

                var successResponse = APIOperationResponse<bool>.Success(true, "Proposal accepted successfully.");
                return StatusCode(successResponse.StatusCode, successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = APIOperationResponse<bool>.Fail(
                    ResponseType.InternalServerError,
                    CommonErrorCodes.ServerError,
                    ex.Message
                );
                return StatusCode(errorResponse.StatusCode, errorResponse);
            }
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.Freelancer)]
        [HttpGet("freelancer-proposals")]
        public async Task<IActionResult> GetProposalsByFreelancer([FromQuery] FreelancerProposalsSpecParams specParams)
        {
            var result = await _proposalService.GetFreelancerProposalsAsync(specParams);
            return Ok(result);
        }
    }
}