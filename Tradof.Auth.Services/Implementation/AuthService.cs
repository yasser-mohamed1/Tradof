using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Tradof.Auth.Services.DTOs;
using Tradof.Auth.Services.Extensions;
using Tradof.Auth.Services.Interfaces;
using Tradof.Auth.Services.Validation;
using Tradof.Common.Enums;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;
using Tradof.EntityFramework.DataBase_Context;
using SystemFile = System.IO.File;

namespace Tradof.Auth.Services.Implementation
{
    public class AuthService(
        IConfiguration _configuration,
        IEmailService _emailService,
        IUserRepository _userRepository,
        UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        IOtpRepository _otpRepository,
        TradofDbContext _context,
        IBackgroundJobClient _backgroundJob,
        IHttpContextAccessor _httpContextAccessor,
        IHttpClientFactory _httpClientFactory,
        IUnitOfWork _unitOfWork) : IAuthService
    {

        private readonly string _jwtSecret = _configuration["JWT:Secret"] ?? Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new Exception("JWT_SECRET is not set.");
        private readonly int _jwtExpiryInMinutes = int.Parse(_configuration["JWT:Expiry"] ?? Environment.GetEnvironmentVariable("JWT_EXPIRY")
            ?? throw new Exception("JWT_EXPIRY is not set."));

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
                    var newCompany = dto.ToCompanyEntity(newUser, _context);
                    await _unitOfWork.Repository<Company>().AddAsync(newCompany);
                    await _unitOfWork.CommitAsync();
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
                    var newFreelancer = dto.ToFreelancerEntity(newUser, _context);
                    await _unitOfWork.Repository<Freelancer>().AddAsync(newFreelancer);
                    await _unitOfWork.CommitAsync();

                    var freelancerLanguagePairs = dto.LanguagePairs.Select(lp => lp.ToFreelancerLanguagesPairEntity(newFreelancer));

                    await _unitOfWork.Repository<FreelancerLanguagesPair>()
                        .AddRangeAsync(freelancerLanguagePairs);
                    await _unitOfWork.CommitAsync();

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
            emailTemplate = emailTemplate.Replace("{{ConfirmationLink}}", $"https://tradof.runasp.net/api/auth/confirm-email?token={newUser.EmailConfirmationToken}&email={newUser.Email}");
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
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmationToken != token)
            {
                return false;
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null!;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<LoginResult> LoginAsync(LoginDto dto)
        {
            ValidationHelper.ValidateLoginDto(dto);
            var user = await _userManager.FindByEmailAsync(dto.Email);
            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (user == null || !isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Check if user is locked out
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Your account is locked. Try again later.");
            }

            if (!user.IsEmailConfirmed)
            {
                throw new UnauthorizedAccessException("Please confirm your email before logging in.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User does not have an assigned role.");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await _userRepository.SaveRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

            var loginResult = new LoginResult
            {
                Token = token,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Role = role
            };

            // Check if user is a Company Employee and get company data
            if (user.UserType == UserType.CompanyEmployee)
            {
                var companyEmployee = await _unitOfWork.Repository<CompanyEmployee>().GetByUserIdAsync(user.Id, new List<System.Linq.Expressions.Expression<Func<CompanyEmployee, object>>>
                {
                    ce => ce.Company
                });

                if (companyEmployee != null)
                {
                    loginResult.CompanyId = companyEmployee.Company.UserId;
                    loginResult.GroupName = companyEmployee.GroupName;
                }
            }
            // Check subscription if user is CompanyAdmin
            //if (role == RoleNames.CompanyAdmin)
            //{
            //    var httpClient = _httpClientFactory.CreateClient();
            //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //    try
            //    {
            //        var response = await httpClient.GetAsync($"https://tradofapi-production.up.railway.app/api/subscription/current-subscription/{user.Id}");
            //        if (response.IsSuccessStatusCode)
            //        {
            //            var content = await response.Content.ReadAsStringAsync();
            //            var subscriptionResponse = JsonSerializer.Deserialize<SubscriptionResponse>(content, new JsonSerializerOptions
            //            {
            //                PropertyNameCaseInsensitive = true
            //            });

            //            if (subscriptionResponse == null || subscriptionResponse.Success == false)
            //            {
            //                throw new UnauthorizedAccessException("You do not have an active subscription.");
            //            }
            //        }
            //        else
            //        {
            //            throw new UnauthorizedAccessException("Subscription service unavailable.");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex);
            //        throw new UnauthorizedAccessException("Error checking subscription.");
            //    }
            //}

            return loginResult;
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("profileImageUrl", user.ProfileImageUrl ?? ""),
                new Claim("userType", user.UserType.ToString()),
            }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<(string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return (null, null);
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Save new refresh token (optional)
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return (newAccessToken, newRefreshToken);
        }

        public async Task ForgetPasswordAsync(ForgetPasswordDto dto)
        {
            ValidationHelper.ValidateForgetPasswordDto(dto);
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser is null)
            {
                throw new ValidationException("Email is not registered.");
            }
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

        private static string GenerateOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            return (BitConverter.ToUInt32(bytes, 0) % 1000000).ToString("D6");
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

        public Task<(string Id, string Role)> GetCurrentUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return Task.FromResult<(string, string)>(("N/A", "N/A"));
            }

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = user.FindFirstValue(ClaimTypes.Role) ?? "No role assigned";

            return Task.FromResult((userId, userRole));
        }

        public async Task ResendOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                throw new ValidationException("Invalid email address.");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is null)
            {
                throw new ValidationException("Email is not registered.");
            }

            var otp = GenerateOtp();
            await _otpRepository.SaveOtpAsync(email, otp, TimeSpan.FromMinutes(10));
            await _emailService.SendEmailAsync(email, "Resend OTP", $"Your OTP is: {otp}");
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public async Task<UserDto?> GetUserById(string id)
        {
            var user = await _context.Users.FindAsync(id);
            return user?.ToUserDto();
        }

        public async Task<(string Token, string RefreshToken, string UserId, string Role)> AuthenticateWithGoogle(HttpContext httpContext)
        {
            var authenticateResult = await httpContext.AuthenticateAsync();

            if (!authenticateResult.Succeeded)
                throw new Exception("Google authentication failed.");

            var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userManager.FindByEmailAsync(email);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User does not have an assigned role.");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await _userRepository.SaveRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

            return (token, refreshToken, user.Id, role);
        }

        public async Task IncreaseProfileViewAsync(string profileUserId)
        {
            var profileUser = await _context.Users.FindAsync(profileUserId);
            if (profileUser == null)
                throw new KeyNotFoundException($"No user with this id: {profileUser}");

            var currentUser = await GetCurrentUserAsync();

            if (currentUser == (null, null) || currentUser.Id == profileUser.Id)
                throw new Exception("The current user can not be the profile user");

            var session = _httpContextAccessor.HttpContext.Session;
            string sessionKey = $"ViewedProfile_{profileUserId}";

            if (session.GetString(sessionKey) == null)
            {
                profileUser.ProfileViews += 1;
                await _context.SaveChangesAsync();
                session.SetString(sessionKey, "true");
            }
        }
    }
}