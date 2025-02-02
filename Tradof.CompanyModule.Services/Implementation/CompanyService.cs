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

        public async Task ChangeCompanyPasswordAsync(ChangeCompanyPasswordDto dto)
        {
            var company = await _context.Companies
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == dto.CompanyId);

            if (company == null) throw new KeyNotFoundException("Company not found");

            if (company.User.PasswordHash != HashPassword(dto.CurrentPassword))
            {
                throw new UnauthorizedAccessException("Current password is incorrect");
            }

            company.User.PasswordHash = HashPassword(dto.NewPassword);

            _context.Update(company);
            await _context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
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

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                newUser.EmailConfirmationToken = emailConfirmationToken;
                _context.Users.Update(newUser);
                await _context.SaveChangesAsync();

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
            emailTemplate = emailTemplate.Replace("{{ConfirmationLink}}", $"http://tradof.runasp.net/api/auth/confirm-email?token={newUser.EmailConfirmationToken}&email={newUser.Email}");
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

        public async Task AddLanguageAsync(string companyId, long languageId)
        {
            var company = await _context.Companies.Include(c => c.PreferredLanguages).FirstOrDefaultAsync(c => c.UserId == companyId);
            if (company == null) throw new KeyNotFoundException("Company not found");
            var language = _context.Languages.Find(languageId);
            if (language is not null)
                company.PreferredLanguages.Add(language);
            else
                throw new KeyNotFoundException("Language not found");
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLanguageAsync(string companyId, long languageId)
        {
            var company = await _context.Companies.Include(c => c.PreferredLanguages).FirstOrDefaultAsync(c => c.UserId == companyId);
            if (company == null) throw new KeyNotFoundException("Company not found");

            var language = company.PreferredLanguages.FirstOrDefault(l => l.Id == languageId);
            if (language != null)
            {
                company.PreferredLanguages.Remove(language);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddSpecializationAsync(string companyId, long specializationId)
        {
            var company = await _context.Companies.Include(c => c.Specializations).FirstOrDefaultAsync(c => c.UserId == companyId);
            if (company == null) throw new KeyNotFoundException("Company not found");
            var specialization = _context.Specializations.Find(specializationId);
            if (specialization is not null)
                company.Specializations.Add(specialization);
            else
                throw new KeyNotFoundException("Specialization not found");

            await _context.SaveChangesAsync();
        }

        public async Task RemoveSpecializationAsync(string companyId, long specializationId)
        {
            var company = await _context.Companies.Include(c => c.Specializations).FirstOrDefaultAsync(c => c.UserId == companyId);
            if (company == null) throw new KeyNotFoundException("Company not found");

            var specialization = company.Specializations.FirstOrDefault(s => s.Id == specializationId);
            if (specialization != null)
            {
                company.Specializations.Remove(specialization);
                await _context.SaveChangesAsync();
            }
        }
    }
}
