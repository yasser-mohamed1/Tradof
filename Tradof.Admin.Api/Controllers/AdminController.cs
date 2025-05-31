using Microsoft.AspNetCore.Mvc;
using Tradof.Admin.Services;
using Tradof.Admin.Services.DataTransferObject.AuthenticationDto;
using Tradof.Admin.Services.DataTransferObject.DashboardDto;
using Tradof.Admin.Services.Helpers;
using Tradof.Admin.Services.Interfaces;
using Tradof.ResponseHandler.Consts;
using Tradof.ResponseHandler.Models;

namespace Tradof.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController(IAuthenticationService _authenticationService,
        IHelperService _helperServic,
        IAdminService _adminService) : ApiControllerBase
    {
        #region AddAdmin
        [Route("AddAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddAdmin(RegisterAdminDto addAdminDto)
        {
            var response = await _authenticationService.AddAdminAsync(addAdminDto);
            return StatusCode(response.StatusCode, response);
        }
        #endregion

        #region Manage Roles
        [Route("CreateRole")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var response = await _authenticationService.CreateRoleAsync(roleName);
            return StatusCode(response.StatusCode, response);
        }

        [Route("DeleteRole")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var response = await _authenticationService.DeleteRoleAsync(roleName);
            return StatusCode(response.StatusCode, response);
        }
        #endregion

        [HttpGet("GetDashboardStatistics")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var statistics = await _adminService.GetDashboardStatisticsAsync();
            return Ok(statistics);
        }

        [HttpGet("GetStatistics")]
        public async Task<IActionResult> GetStatistics(int? year = null)
        {
            try
            {
                var statistics = await _adminService.GetStatisticsAsync(year);

                var response = APIOperationResponse<List<MonthlyStatisticsDto>>.Success(
                    statistics,
                    "Statistics retrieved successfully."
                );

                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIOperationResponse<List<MonthlyStatisticsDto>>.Fail(
                    ResponseType.InternalServerError,
                    CommonErrorCodes.FailedToRetrieveData,
                    $"An error occurred while retrieving statistics: {ex.Message}"
                );

                return StatusCode(errorResponse.StatusCode, errorResponse);
            }
        }

        [HttpGet("GetFreelancersAndCompanies")]
        public async Task<IActionResult> GetFreelancersAndCompanies()
        {
            var users = await _adminService.GetFreelancersAndCompaniesAsync();
            return Ok(users);
        }

        [HttpPost("ToggleBlockStatus")]
        public async Task<IActionResult> ToggleBlockStatus(string userId, bool isBlocked)
        {
            var response = await _adminService.ToggleBlockStatusAsync(userId, isBlocked);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAdminsAsync();
            return Ok(admins);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAdmin(string userId)
        {
            var response = await _authenticationService.DeleteAdminAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}