using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Tradof.Admin.Services.DataTransferObject.AuthenticationDto;
using Tradof.Admin.Services.DataTransferObject.DashboardDto;
using Tradof.Admin.Services.Extensions;
using Tradof.Admin.Services.Interfaces;
using Tradof.Common.Enums;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;
using Tradof.ResponseHandler.Models;

namespace Tradof.Admin.Services
{
    public class AdminService(TradofDbContext _context,
         UserManager<ApplicationUser> _userManager) : IAdminService
    {
        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            var numberOfProjects = await _context.Projects.CountAsync();
            var numberOfFreelancers = await _userManager.Users.CountAsync(u => u.UserType == UserType.Freelancer);
            var numberOfCompanies = await _userManager.Users.CountAsync(u => u.UserType == UserType.Company);
            var numberOfAdmins = await _userManager.Users.CountAsync(u => u.UserType == UserType.Admin);
            var numberOfAdminsOnline = await _userManager.Users.CountAsync(u => u.UserType == UserType.Admin && u.LockoutEnd == null);

            return new DashboardStatisticsDto
            {
                NumberOfProjects = numberOfProjects,
                NumberOfFreelancers = numberOfFreelancers,
                NumberOfCompanies = numberOfCompanies,
                NumberOfAdmins = numberOfAdmins,
                NumberOfAdminsOnline = numberOfAdminsOnline
            };
        }

        public async Task<List<MonthlyStatisticsDto>> GetStatisticsAsync(int? year = null)
        {
            var result = new List<MonthlyStatisticsDto>();

            if (!year.HasValue)
                year = DateTime.UtcNow.Year;

            for (int i = 1; i <= 12; i++)
            {
                var companiesCount = await _context.Companies
                    .CountAsync(c => c.CreationDate.Year == year && c.CreationDate.Month == i);

                var freelancersCount = await _context.Freelancers
                    .CountAsync(f => f.CreationDate.Year == year && f.CreationDate.Month == i);

                var projectStats = await _context.Projects
                    .Where(p => p.CreationDate.Year == year && p.CreationDate.Month == i)
                    .GroupBy(p => 1)
                    .Select(g => new
                    {
                        Count = g.Count(),
                        TotalCost = g.Sum(p => p.MaxPrice)
                    })
                    .FirstOrDefaultAsync();

                result.Add(new MonthlyStatisticsDto
                {
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    Companies = companiesCount,
                    Freelancer = freelancersCount,
                    Projects = new ProjectStat
                    {
                        Number = projectStats?.Count ?? 0,
                        Cost = projectStats?.TotalCost ?? 0
                    }
                });
            }

            return result;
        }

        public async Task<List<GetUserDto>> GetFreelancersAndCompaniesAsync()
        {
            var users = await _userManager.Users
                .Where(u => u.UserType == UserType.Freelancer || u.UserType == UserType.CompanyAdmin)
                .ToListAsync();

            return users.Select(u => u.ToGetUserDto()).ToList();
        }

        public async Task<APIOperationResponse<object>> ToggleBlockStatusAsync(string userId, bool isBlocked, int? blockDurationInMinutes = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return APIOperationResponse<object>.NotFound("User not found");
            }

            if (isBlocked)
            {
                if (blockDurationInMinutes.HasValue && blockDurationInMinutes > 0)
                {
                    user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(blockDurationInMinutes.Value); // Temporary block
                }
                else
                {
                    user.LockoutEnd = DateTimeOffset.MaxValue; // Permanent block
                }
            }
            else
            {
                user.LockoutEnd = null; // Unblock user
            }

            await _userManager.UpdateAsync(user);

            return APIOperationResponse<object>.Success(
                new { userId, isBlocked, lockoutEnd = user.LockoutEnd },
                isBlocked
                    ? (blockDurationInMinutes.HasValue ? $"User has been blocked for {blockDurationInMinutes} minutes" : "User has been permanently blocked")
                    : "User has been unblocked"
            );
        }

        public async Task<List<GetUserDto>> GetAllAdminsAsync()
        {
            var admins = await _userManager.Users
                .Where(u => u.UserType == UserType.Admin)
                .ToListAsync();

            return admins.Select(u => u.ToGetUserDto()).ToList();
        }
    }
}