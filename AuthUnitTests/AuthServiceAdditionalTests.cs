using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.ComponentModel.DataAnnotations;
using Tradof.Auth.Services.DTOs;
using Tradof.Auth.Services.Implementation;
using Tradof.Auth.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Tradof.Common.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AuthUnitTests
{
    public class AuthServiceAdditionalTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<IFreelancerRepository> _mockFreelancerRepository;
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly Mock<IOtpRepository> _mockOtpRepository;
        private readonly Mock<IFreelancerLanguagesPairRepository> _mockFreelancerLanguagesPairRepository;
        private readonly TradofDbContext _mockContext;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJob;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly AuthService _authService;

        public AuthServiceAdditionalTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockEmailService = new Mock<IEmailService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockUserManager = MockUserManager();
            _mockRoleManager = MockRoleManager();
            _mockFreelancerRepository = new Mock<IFreelancerRepository>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mockOtpRepository = new Mock<IOtpRepository>();
            _mockFreelancerLanguagesPairRepository = new Mock<IFreelancerLanguagesPairRepository>();
            _mockBackgroundJob = new Mock<IBackgroundJobClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Setup configuration
            _mockConfiguration.Setup(x => x["JWT:Secret"]).Returns("your-256-bit-secret-your-256-bit-secret");
            _mockConfiguration.Setup(x => x["JWT:Expiry"]).Returns("60");

            // Create DbContext with in-memory database
            var options = new DbContextOptionsBuilder<TradofDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _mockContext = new TradofDbContext(options);

            _authService = new AuthService(
                _mockConfiguration.Object,
                _mockEmailService.Object,
                _mockUserRepository.Object,
                _mockRoleRepository.Object,
                _mockUserManager.Object,
                _mockRoleManager.Object,
                _mockFreelancerRepository.Object,
                _mockCompanyRepository.Object,
                _mockOtpRepository.Object,
                _mockFreelancerLanguagesPairRepository.Object,
                _mockContext,
                _mockBackgroundJob.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Fact]
        public async Task ForgetPasswordAsync_WithValidEmail_SendsOtp()
        {
            // Arrange
            var forgetPasswordDto = new ForgetPasswordDto(
                Email: "test@example.com"
            );

            var user = new ApplicationUser
            {
                Id = "testUserId",
                Email = forgetPasswordDto.Email,
                UserName = forgetPasswordDto.Email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(forgetPasswordDto.Email))
                .ReturnsAsync(user);

            // Act
            await _authService.ForgetPasswordAsync(forgetPasswordDto);

            // Assert
            _mockOtpRepository.Verify(x => x.SaveOtpAsync(
                It.Is<string>(e => e == forgetPasswordDto.Email),
                It.IsAny<string>(),
                It.IsAny<TimeSpan>()), Times.Once);
            _mockEmailService.Verify(x => x.SendEmailAsync(
                It.Is<string>(e => e == forgetPasswordDto.Email),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ForgetPasswordAsync_WithInvalidEmail_ThrowsValidationException()
        {
            // Arrange
            var forgetPasswordDto = new ForgetPasswordDto(
                Email: "nonexistent@example.com"
            );

            _mockUserManager.Setup(x => x.FindByEmailAsync(forgetPasswordDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _authService.ForgetPasswordAsync(forgetPasswordDto));

            Assert.Contains("Email is not registered", exception.Message);
        }

        [Fact]
        public async Task VerifyOtpAsync_WithValidOtp_ReturnsTrue()
        {
            // Arrange
            var otpVerificationDto = new OtpVerificationDto(
                Email: "test@example.com",
                Otp: "123456"
            );

            _mockOtpRepository.Setup(x => x.ValidateOtpAsync(otpVerificationDto.Email, otpVerificationDto.Otp))
                .ReturnsAsync(true);

            // Act
            await _authService.VerifyOtpAsync(otpVerificationDto);

            // Assert
            _mockOtpRepository.Verify(x => x.ValidateOtpAsync(
                It.Is<string>(e => e == otpVerificationDto.Email),
                It.Is<string>(o => o == otpVerificationDto.Otp)), Times.Once);
        }

        [Fact]
        public async Task VerifyOtpAsync_WithInvalidOtp_ThrowsValidationException()
        {
            // Arrange
            var otpVerificationDto = new OtpVerificationDto(
                Email: "test@example.com",
                Otp: "wrongotp"
            );

            _mockOtpRepository.Setup(x => x.ValidateOtpAsync(otpVerificationDto.Email, otpVerificationDto.Otp))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _authService.VerifyOtpAsync(otpVerificationDto));

            Assert.Contains("Invalid or expired OTP", exception.Message);
        }

        [Fact]
        public async Task ChangePasswordWithTokenAsync_WithValidToken_ChangesPassword()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                Email = "test@example.com",
                Token = "validtoken",
                NewPassword = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            var user = new ApplicationUser
            {
                Id = "testUserId",
                Email = changePasswordDto.Email,
                UserName = changePasswordDto.Email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(changePasswordDto.Email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, changePasswordDto.Token, changePasswordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _authService.ChangePasswordWithTokenAsync(changePasswordDto);

            // Assert
            _mockUserManager.Verify(x => x.ResetPasswordAsync(
                It.IsAny<ApplicationUser>(),
                It.Is<string>(t => t == changePasswordDto.Token),
                It.Is<string>(p => p == changePasswordDto.NewPassword)), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordWithTokenAsync_WithInvalidToken_ThrowsValidationException()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                Email = "test@example.com",
                Token = "invalidtoken",
                NewPassword = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            var user = new ApplicationUser
            {
                Id = "testUserId",
                Email = changePasswordDto.Email,
                UserName = changePasswordDto.Email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(changePasswordDto.Email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, changePasswordDto.Token, changePasswordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _authService.ChangePasswordWithTokenAsync(changePasswordDto));

            Assert.Contains("Invalid token", exception.Message);
        }

        [Fact]
        public async Task ResendOtpAsync_WithValidEmail_SendsNewOtp()
        {
            // Arrange
            var email = "test@example.com";
            var user = new ApplicationUser
            {
                Id = "testUserId",
                Email = email,
                UserName = email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            await _authService.ResendOtpAsync(email);

            // Assert
            _mockOtpRepository.Verify(x => x.SaveOtpAsync(
                It.Is<string>(e => e == email),
                It.IsAny<string>(),
                It.IsAny<TimeSpan>()), Times.Once);
            _mockEmailService.Verify(x => x.SendEmailAsync(
                It.Is<string>(e => e == email),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ResendOtpAsync_WithInvalidEmail_ThrowsValidationException()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _authService.ResendOtpAsync(email));

            Assert.Contains("Email is not registered", exception.Message);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());
            return mgr;
        }

        private static Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            var mgr = new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
            return mgr;
        }
    }
} 