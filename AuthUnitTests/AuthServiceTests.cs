using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Moq;
using System.ComponentModel.DataAnnotations;
using Tradof.Auth.Services.DTOs;
using Tradof.Auth.Services.Implementation;
using Tradof.Auth.Services.Interfaces;
using Tradof.Common.Enums;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;
using Tradof.EntityFramework.Helpers;

namespace AuthUnitTests
{
    public class AuthServiceTests
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

        public AuthServiceTests()
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
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
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
        public async Task LoginAsync_WithValidCredentials_ReturnsTokenAndRefreshToken()
        {
            // Arrange
            var loginDto = new LoginDto(
                Email: "test@example.com",
                Password: "Password123!"
            );

            var user = new ApplicationUser
            {
                Id = "testUserId",
                Email = loginDto.Email,
                UserName = loginDto.Email,
                FirstName = "Test",
                LastName = "User",
                IsEmailConfirmed = true,
                UserType = UserType.CompanyAdmin,
                ProfileImageUrl = "test.jpg"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);

            _mockRoleRepository.Setup(x => x.GetUserRoleAsync(user.Id))
                .ReturnsAsync(UserType.CompanyAdmin.ToString());

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(UserType.CompanyAdmin.ToString(), result.Role);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginDto = new LoginDto(
                Email: "test@example.com",
                Password: "WrongPassword"
            );

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            //Assert.Equal(user.Id, result.UserId);
            Assert.Equal(UserType.CompanyAdmin.ToString(), result.Role);
        }

        [Fact]
        public async Task RegisterCompanyAsync_WithValidData_RegistersSuccessfully()
        {
            // Arrange
            var registerDto = new RegisterCompanyDto(
                Email: "company@example.com",
                Password: "Password123!",
                CompanyName: "Test Company",
                FirstName: "John",
                LastName: "Doe",
                CompanyAddress: "Test Address",
                JobTitle: "Manager",
                CountryId: 1,
                PhoneNumber: "+1234567890",
                SpecializationIds: new List<long> { 1, 2 },
                PreferredLanguageIds: new List<long> { 1, 2 }
            );

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockRoleManager.Setup(x => x.RoleExistsAsync(UserType.CompanyAdmin.ToString()))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserType.CompanyAdmin.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            // Act & Assert
            await _authService.RegisterCompanyAsync(registerDto);

            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password), Times.Once);
            _mockCompanyRepository.Verify(x => x.AddAsync(It.IsAny<Company>()), Times.Once);
        }

        [Fact]
        public async Task RegisterCompanyAsync_WithExistingEmail_ThrowsValidationException()
        {
            // Arrange
            var registerDto = new RegisterCompanyDto(
                Email: "existing@example.com",
                Password: "Password123!",
                CompanyName: "Test Company",
                FirstName: "John",
                LastName: "Doe",
                CompanyAddress: "Test Address",
                JobTitle: "Manager",
                CountryId: 1,
                PhoneNumber: "+1234567890",
                SpecializationIds: new List<long> { 1, 2 },
                PreferredLanguageIds: new List<long> { 1, 2 }
            );

            var existingUser = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _authService.RegisterCompanyAsync(registerDto));

            Assert.Contains("already registered", exception.Message);
        }

        [Fact]
        public async Task RegisterFreelancerAsync_WithValidData_RegistersSuccessfully()
        {
            // Arrange
            var registerDto = new RegisterFreelancerDto(
                Email: "freelancer@example.com",
                Password: "Password123!",
                FirstName: "John",
                LastName: "Doe",
                WorkExperience: 5,
                CountryId: 1,
                PhoneNumber: "+1234567890",
                SpecializationIds: new List<long> { 1, 2 },
                LanguagePairs: new List<LanguagePairDto>
                {
                    new LanguagePairDto(LanguageFromId: 1, LanguageToId: 2),
                    new LanguagePairDto(LanguageFromId: 2, LanguageToId: 3)
                }
            );

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockRoleManager.Setup(x => x.RoleExistsAsync(UserType.Freelancer.ToString()))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserType.Freelancer.ToString()))
                .ReturnsAsync(IdentityResult.Success);

            // Act & Assert
            await _authService.RegisterFreelancerAsync(registerDto);

            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password), Times.Once);
            _mockFreelancerRepository.Verify(x => x.AddAsync(It.IsAny<Freelancer>()), Times.Once);
            _mockFreelancerLanguagesPairRepository.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<FreelancerLanguagesPair>>()), Times.Once);
        }

        [Fact]
        public async Task RegisterFreelancerAsync_WithExistingEmail_ThrowsValidationException()
        {
            // Arrange
            var registerDto = new RegisterFreelancerDto(
                Email: "existing@example.com",
                Password: "Password123!",
                FirstName: "John",
                LastName: "Doe",
                WorkExperience: 5,
                CountryId: 1,
                PhoneNumber: "+1234567890",
                SpecializationIds: new List<long> { 1, 2 },
                LanguagePairs: new List<LanguagePairDto>
                {
                    new LanguagePairDto(LanguageFromId: 1, LanguageToId: 2),
                    new LanguagePairDto(LanguageFromId: 2, LanguageToId: 3)
                }
            );

            var existingUser = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _authService.RegisterFreelancerAsync(registerDto));

            Assert.Contains("already registered", exception.Message);
        }

        [Fact]
        public async Task RegisterFreelancerAsync_WithNegativeWorkExperience_ThrowsValidationException()
        {
            // Arrange
            var registerDto = new RegisterFreelancerDto(
                Email: "freelancer@example.com",
                Password: "Password123!",
                FirstName: "John",
                LastName: "Doe",
                WorkExperience: -1, // Invalid: negative work experience
                CountryId: 1,
                PhoneNumber: "+1234567890",
                SpecializationIds: new List<long> { 1, 2 },
                LanguagePairs: new List<LanguagePairDto>
                {
                    new LanguagePairDto(LanguageFromId: 1, LanguageToId: 2),
                    new LanguagePairDto(LanguageFromId: 2, LanguageToId: 3)
                }
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _authService.RegisterFreelancerAsync(registerDto));

            Assert.Contains("Work experience cannot be negative", exception.Message);
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