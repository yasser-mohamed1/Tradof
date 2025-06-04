using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Tradof.Common.Enums;
using Tradof.CompanyModule.Services.DTOs;
using Tradof.CompanyModule.Services.Extensions;
using Tradof.CompanyModule.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;
using SystemFile = System.IO.File;

namespace Tradof.CompanyModule.Services.Implementation
{
    public class CompanyService(
        TradofDbContext _context,
        IEmailService _emailService,
        UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        IBackgroundJobClient _backgroundJob) : ICompanyService
    {
        public async Task<CompanyDto?> GetByIdAsync(string id)
        {
            var company = await _context.Companies
                .Include(c => c.Specializations)
                .Include(c => c.PreferredLanguages)
                .Include(c => c.Medias)
                .Include(c => c.Employees)
                .Include(c => c.User)
                .Include(c => c.Projects)
                .ThenInclude(p => p.Ratings)
                .FirstOrDefaultAsync(c => c.UserId == id);

            return company?.ToDto();
        }

        public async Task<CompanySubscriptionDto?> GetCurrentSubscriptionAsync(string userId)
        {
            var company = await _context.Companies
                .Include(c => c.Subscriptions)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (company == null)
            {
                return null;
            }

            var currentSubscription = company.Subscriptions
                .FirstOrDefault(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

            return currentSubscription?.ToDto();
        }

        public async Task<bool> ChangeCompanyPasswordAsync(string Id, ChangeCompanyPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword) return false;

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded;
        }

        public async Task AddEmployeeAsync(CreateCompanyEmployeeDto dto)
        {
            var newUser = dto.ToApplicationUser();

            await RegisterUserAsync(newUser, dto.Password, dto.Email, UserType.CompanyEmployee.ToString(), async user =>
            {
                var Company = _context.Companies.FirstOrDefault(c => c.UserId == dto.CompanyId);
                var employee = dto.ToCompanyEmployee(user.Id, Company!);

                _context.CompanyEmployees.Add(employee);
                await _context.SaveChangesAsync();
            });
        }

        public async Task<IEnumerable<EmployeeDto>> GetCompanyEmployeesAsync(string companyId)
        {
            var company = await _context.Companies
               .Include(c => c.Employees)
                   .ThenInclude(e => e.Country)
               .Include(c => c.Employees)
                   .ThenInclude(e => e.User)
               .FirstOrDefaultAsync(c => c.UserId == companyId);

            if (company == null) return Enumerable.Empty<EmployeeDto>();

            return company.Employees.Select(e => new EmployeeDto(
                e.UserId,
                $"{e.User.FirstName} {e.User.LastName}",
                e.JobTitle,
                e.User.Email ?? "N/A",
                e.User.PhoneNumber ?? "N/A",
                e.GroupName.ToString(),
                e.Country?.Name ?? "N/A"
            )).ToList();
        }

        private async Task RegisterUserAsync(ApplicationUser newUser, string password, string email, string roleName, Func<ApplicationUser, Task> additionalEntityAction)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    throw new ValidationException("Email is already registered.");
                }

                var result = await _userManager.CreateAsync(newUser, password);
                if (!result.Succeeded)
                {
                    throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                await AssignRoleAsync(newUser, roleName);

                await additionalEntityAction(newUser);

                _backgroundJob.Enqueue(() => SendConfirmationEmailAsync(newUser));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task AssignRoleAsync(ApplicationUser user, string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new ValidationException($"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        public async Task SendConfirmationEmailAsync(ApplicationUser newUser)
        {
            string templatePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Templates", "ConfirmEmail.html");
            string emailTemplate = await SystemFile.ReadAllTextAsync(templatePath);
            emailTemplate = emailTemplate.Replace("{{FirstName}}", newUser.FirstName);
            emailTemplate = emailTemplate.Replace("{{ConfirmationLink}}", $"https://tradof.runasp.net/api/auth/confirm-email?token={newUser.EmailConfirmationToken}&email={newUser.Email}");
            emailTemplate = emailTemplate.Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

            await _emailService.SendEmailAsync(newUser.Email!, "Confirm Your Email", emailTemplate);
        }

        public async Task UpdateCompanyAsync(UpdateCompanyDto dto)
        {
            var company = await _context.Companies
                .Include(c => c.User)
                .Include(c => c.Specializations)
                .Include(c => c.PreferredLanguages)
                .FirstOrDefaultAsync(c => c.UserId == dto.Id);

            if (company == null) throw new KeyNotFoundException("Company not found");

            dto.UpdateEntity(company);
            _context.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task AddLanguagesAsync(string companyId, IEnumerable<long> languageIds)
        {
            var company = await _context.Companies.Include(c => c.PreferredLanguages)
                .FirstOrDefaultAsync(c => c.UserId == companyId);

            if (company == null) throw new KeyNotFoundException("Company not found");

            var languages = await _context.Languages
                .Where(l => languageIds.Contains(l.Id))
                .ToListAsync();

            if (!languages.Any()) throw new KeyNotFoundException("No valid languages found");

            foreach (var language in languages)
            {
                if (!company.PreferredLanguages.Contains(language))
                {
                    company.PreferredLanguages.Add(language);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveLanguagesAsync(string companyId, IEnumerable<long> languageIds)
        {
            var company = await _context.Companies.Include(c => c.PreferredLanguages)
                .FirstOrDefaultAsync(c => c.UserId == companyId);

            if (company == null) throw new KeyNotFoundException("Company not found");

            var languagesToRemove = company.PreferredLanguages
                .Where(l => languageIds.Contains(l.Id))
                .ToList();

            if (!languagesToRemove.Any()) throw new KeyNotFoundException("No matching languages found to remove");

            foreach (var language in languagesToRemove)
            {
                company.PreferredLanguages.Remove(language);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddSpecializationsAsync(string companyId, IEnumerable<long> specializationIds)
        {
            var company = await _context.Companies.Include(c => c.Specializations)
                .FirstOrDefaultAsync(c => c.UserId == companyId);

            if (company == null) throw new KeyNotFoundException("Company not found");

            var specializations = await _context.Specializations
                .Where(s => specializationIds.Contains(s.Id))
                .ToListAsync();

            if (!specializations.Any()) throw new KeyNotFoundException("No valid specializations found");

            foreach (var specialization in specializations)
            {
                if (!company.Specializations.Contains(specialization))
                {
                    company.Specializations.Add(specialization);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveSpecializationsAsync(string companyId, IEnumerable<long> specializationIds)
        {
            var company = await _context.Companies.Include(c => c.Specializations)
                .FirstOrDefaultAsync(c => c.UserId == companyId);

            if (company == null) throw new KeyNotFoundException("Company not found");

            var specializationsToRemove = company.Specializations
                .Where(s => specializationIds.Contains(s.Id))
                .ToList();

            if (!specializationsToRemove.Any()) throw new KeyNotFoundException("No matching specializations found to remove");

            foreach (var specialization in specializationsToRemove)
            {
                company.Specializations.Remove(specialization);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateOrRemoveSocialMediasAsync(string id, IEnumerable<CreateSocialMediaDto> socialMedias)
        {
            var company = await _context.Set<Company>()
                .Include(c => c.Medias)
                .FirstOrDefaultAsync(c => c.UserId == id);

            if (company == null)
                throw new ArgumentException("Invalid Company ID.");

            var mediaPlatformTypes = socialMedias.Select(sm => sm.PlatformType.ToLower()).ToList();

            // Remove medias with empty links
            var mediasToRemove = company.Medias
                .Where(m => socialMedias.Any(dto => dto.PlatformType.Equals(m.PlatformType.ToString(), StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(dto.Link)))
                .ToList();

            if (mediasToRemove.Any())
            {
                _context.RemoveRange(mediasToRemove);
            }

            // Update existing or add new medias
            foreach (var dto in socialMedias)
            {
                if (!Enum.TryParse<PlatformType>(dto.PlatformType, true, out var platformType))
                    throw new ArgumentException($"Invalid PlatformType: {dto.PlatformType}");

                if (string.IsNullOrWhiteSpace(dto.Link)) continue;

                var existingMedia = company.Medias.FirstOrDefault(m => m.PlatformType == platformType);

                if (existingMedia != null)
                {
                    existingMedia.Link = dto.Link;
                }
                else
                {
                    var socialMediaEntity = company.ToSocialMedia(dto);
                    company.Medias.Add(socialMediaEntity);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<CompanyDto?> GetByEmployeeIdAsync(string employeeId)
        {
            var company = await _context.Companies
                .Include(c => c.Specializations)
                .Include(c => c.PreferredLanguages)
                .Include(c => c.Medias)
                .Include(c => c.Employees)
                .Include(c => c.User)
                .Include(c => c.Projects)
                    .ThenInclude(p => p.Ratings)
                .FirstOrDefaultAsync(c => c.Employees.Any(e => e.UserId == employeeId));

            return company?.ToDto();
        }
    }
}