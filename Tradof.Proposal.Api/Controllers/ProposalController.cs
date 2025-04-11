using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        public async Task<IActionResult> GetProposalsCountByMonth(int? year = null, int? month = null, ProposalStatus? status = ProposalStatus.Pending)
        {
            try
            {
                var count = await _proposalService.GetProposalsCountByMonth(year, month, status);
                var response = APIOperationResponse<int>.Success(count, "Proposals count retrieved successfully.");

                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIOperationResponse<int>.Fail(
                    ResponseType.InternalServerError,
                    CommonErrorCodes.FailedToRetrieveData,
                    ex.Message
                );
                return StatusCode(errorResponse.StatusCode, errorResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProposalDto projectDto)
        {
            try
            {
                var result = await _proposalService.CreateAsync(projectDto);

                if (result != null)
                {
                    var response = APIOperationResponse<ProposalDto>.Success(result, "Proposal created successfully.");
                    return StatusCode(response.StatusCode, response);
                }
                else
                {
                    var errorResponse = APIOperationResponse<CreateProposalDto>.Fail(
                        ResponseType.BadRequest,
                        CommonErrorCodes.FailedToSaveData,
                        "Failed to create proposal."
                    );
                    return StatusCode(errorResponse.StatusCode, errorResponse);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = APIOperationResponse<CreateProposalDto>.Fail(
                    ResponseType.InternalServerError,
                    CommonErrorCodes.FailedToSaveData,
                    $"An error occurred while creating the proposal: {ex.Message}"
                );
                return StatusCode(errorResponse.StatusCode, errorResponse);
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
            try
            {
                var result = await _proposalService.DenyProposal(projectId, ProposalId);

                if (!result)
                {
                    var failedResponse = APIOperationResponse<bool>.Fail(
                        ResponseType.BadRequest,
                        CommonErrorCodes.OperationFailed,
                        "Proposal denial failed."
                    );
                    return StatusCode(failedResponse.StatusCode, failedResponse);
                }

                var successResponse = APIOperationResponse<bool>.Success(true, "Proposal denied successfully.");
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

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel(long ProposalId)
        {
            try
            {
                var result = await _proposalService.CancelProposal(ProposalId);

                if (!result)
                {
                    var failedResponse = APIOperationResponse<bool>.Fail(
                        ResponseType.BadRequest,
                        CommonErrorCodes.OperationFailed,
                        "Proposal cancellation failed."
                    );
                    return StatusCode(failedResponse.StatusCode, failedResponse);
                }

                var successResponse = APIOperationResponse<bool>.Success(true, "Proposal cancelled successfully.");
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.Freelancer)]
        [HttpGet("freelancer-proposals")]
        public async Task<IActionResult> GetProposalsByFreelancer([FromQuery] FreelancerProposalsSpecParams specParams)
        {
            var result = await _proposalService.GetFreelancerProposalsAsync(specParams);
            return Ok(result);
        }
    }
}