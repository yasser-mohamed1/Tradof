using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<StatisticsDto> GetStatisticsAsync()
        {
            var statistics = new StatisticsDto();
            var today = DateTime.UtcNow;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfYear = new DateTime(today.Year, 1, 1);

            // *** YEARLY DATA ***
            var freelancersByMonth = await _context.Freelancers
                .Where(f => f.CreationDate >= startOfMonth)
                .GroupBy(f => f.CreationDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            var projectsByMonth = await _context.Projects
                .Where(p => p.CreationDate >= startOfYear)
                .GroupBy(p => p.CreationDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            var companiesByMonth = await _context.Companies
                .Where(c => c.CreationDate >= startOfMonth)
                .GroupBy(c => c.CreationDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var item in freelancersByMonth)
                statistics.FreelancersPerMonth[item.Month - 1] = item.Count;

            foreach (var item in projectsByMonth)
                statistics.ProjectsPerMonth[item.Month - 1] = item.Count;

            foreach (var item in companiesByMonth)
                statistics.CompaniesPerMonth[item.Month - 1] = item.Count;


            // *** MONTHLY DATA (DAY-WISE) ***
            var freelancersByDay = await _context.Freelancers
                .Where(f => f.CreationDate >= startOfWeek)
                .GroupBy(u => u.CreationDate.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            var projectsByDay = await _context.Projects
                .Where(p => p.CreationDate >= startOfMonth)
                .GroupBy(p => p.CreationDate.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            var companiesByDay = await _context.Projects
                .Where(p => p.CreationDate >= startOfMonth)
                .GroupBy(u => u.CreationDate.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var item in freelancersByDay)
                statistics.FreelancersPerDay[item.Day - 1] = item.Count;

            foreach (var item in projectsByDay)
                statistics.ProjectsPerDay[item.Day - 1] = item.Count;

            foreach (var item in companiesByDay)
                statistics.CompaniesPerDay[item.Day - 1] = item.Count;


            // *** WEEKLY DATA (LAST 7 DAYS) ***
            var freelancersByWeek = await _context.Freelancers
                .Where(f => f.CreationDate >= startOfWeek)
                .GroupBy(f => f.CreationDate.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            var projectsByWeek = await _context.Projects
                .Where(p => p.CreationDate >= startOfWeek)
                .GroupBy(p => p.CreationDate.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            var companiesByWeek = await _context.Companies
                .Where(c => c.CreationDate >= startOfMonth)
                .GroupBy(c => c.CreationDate.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var item in freelancersByWeek)
                statistics.FreelancersPerWeek[item.Day - startOfWeek.Day] = item.Count;

            foreach (var item in projectsByWeek)
                statistics.ProjectsPerWeek[item.Day - startOfWeek.Day] = item.Count;

            foreach (var item in companiesByWeek)
                statistics.CompaniesPerWeek[item.Day - startOfWeek.Day] = item.Count;

            return statistics;
        }

        public async Task<List<GetUserDto>> GetFreelancersAndCompaniesAsync()
        {
            var users = await _userManager.Users
                .Where(u => u.UserType == UserType.Freelancer || u.UserType == UserType.Company)
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