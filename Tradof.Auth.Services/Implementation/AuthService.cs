using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tradof.Auth.Services.DTOs;
using Tradof.Auth.Services.Extensions;
using Tradof.Auth.Services.Interfaces;
using Tradof.Auth.Services.Validation;
using Tradof.Common.Enums;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;
using SystemFile = System.IO.File;

namespace Tradof.Auth.Services.Implementation
{
    public class AuthService(
        IEmailService _emailService,
        IUserRepository _userRepository,
        IRoleRepository _roleRepository,
        UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        IFreelancerRepository _freelancerRepository,
        ICompanyRepository _companyRepository,
        IOtpRepository _otpRepository,
        IFreelancerLanguagesPairRepository _freelancerLanguagesPairRepository,
        TradofDbContext _context,
        IBackgroundJobClient _backgroundJob) : IAuthService
    {


        private readonly string _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new Exception("JWT_SECRET is not set.");
        private readonly int _jwtExpiryInMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRY") ?? throw new Exception("JWT_EXPIRY is not set."));

        public async Task RegisterCompanyAsync(RegisterCompanyDto dto)
        {
            ValidationHelper.ValidateRegisterCompanyDto(dto);

            await RegisterUserAsync(
                newUser: dto.ToApplicationUserEntity(),
                password: dto.Password,
                email: dto.Email,
                roleName: UserType.CompanyAdmin.ToString(),
                additionalEntityAction: async newUser =>
                {
                    var newCompany = dto.ToCompanyEntity(newUser);
                    await _companyRepository.AddAsync(newCompany);

                    _backgroundJob.Enqueue(() => SendConfirmationEmailAsync(newUser));
                });
        }

        public async Task RegisterFreelancerAsync(RegisterFreelancerDto dto)
        {
            ValidationHelper.ValidateRegisterFreelancerDto(dto);

            await RegisterUserAsync(
                newUser: dto.ToApplicationUserEntity(),
                password: dto.Password,
                email: dto.Email,
                roleName: UserType.Freelancer.ToString(),
                additionalEntityAction: async newUser =>
                {
                    var newFreelancer = dto.ToFreelancerEntity(newUser);
                    await _freelancerRepository.AddAsync(newFreelancer);

                    var freelancerLanguagePairs = dto.LanguagePairs.Select(lp => lp.ToFreelancerLanguagesPairEntity(newFreelancer));

                    await _freelancerLanguagesPairRepository.AddRangeAsync(freelancerLanguagePairs);

                    _backgroundJob.Enqueue(() => SendConfirmationEmailAsync(newUser));
                });
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

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
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

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || user.EmailConfirmationToken != token)
            {
                return false;
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null!;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<(string Token, string UserId, string Role)> LoginAsync(LoginDto dto)
        {
            ValidationHelper.ValidateLoginDto(dto);

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash, user))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!user.IsEmailConfirmed)
            {
                throw new UnauthorizedAccessException("Please confirm your email before logging in.");
            }

            var role = await _roleRepository.GetUserRoleAsync(user.Id);
            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User does not have an assigned role.");
            }

            var token = GenerateJwtToken(user);
            return (Token: token, UserId: user.Id, Role: role);
        }

        public async Task ForgetPasswordAsync(ForgetPasswordDto dto)
        {
            ValidationHelper.ValidateForgetPasswordDto(dto);

            var otp = GenerateOtp();
            await _otpRepository.SaveOtpAsync(dto.Email, otp, TimeSpan.FromMinutes(10));
            await _emailService.SendEmailAsync(dto.Email, "Reset Your Password", $"Your OTP is: {otp}");
        }

        public async Task VerifyOtpAsync(OtpVerificationDto dto)
        {
            ValidationHelper.ValidateOtpVerificationDto(dto);

            var isOtpValid = await _otpRepository.ValidateOtpAsync(dto.Email, dto.Otp);
            if (!isOtpValid)
            {
                throw new ValidationException("Invalid or expired OTP.");
            }
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType.ToString())
                ]),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateOtp()
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            return (BitConverter.ToUInt32(bytes, 0) % 1000000).ToString("D6");
        }

        public static bool VerifyPassword(string password, string storedHash, ApplicationUser user)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var result = passwordHasher.VerifyHashedPassword(user, storedHash, password);

            return result == PasswordVerificationResult.Success;
        }

        public async Task ChangePasswordWithTokenAsync(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) throw new ValidationException("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new ValidationException("User not found.");

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

    }
}