using Tradof.Admin.Services.DataTransferObject.AuthenticationDto;
using Tradof.Admin.Services.DataTransferObject.DashboardDto;
using Tradof.Data.Entities;
using Tradof.ResponseHandler.Models;

namespace Tradof.Admin.Services.Interfaces
{
    public interface IAdminService
    {
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
        Task<List<MonthlyStatisticsDto>> GetStatisticsAsync(int? year);
        Task<List<GetUserDto>> GetFreelancersAndCompaniesAsync();
        Task<APIOperationResponse<object>> ToggleBlockStatusAsync(string userId, bool isBlocked, int? blockDurationInMinutes = null);
        Task<List<GetUserDto>> GetAllAdminsAsync();
    }
}
