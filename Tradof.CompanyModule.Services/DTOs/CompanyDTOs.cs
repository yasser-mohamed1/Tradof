using System.ComponentModel.DataAnnotations;

namespace Tradof.CompanyModule.Services.DTOs
{
    public record CompanyDto(
        string Id,
        string CompanyAddress,
        string CompanyName,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string JobTitle,
        int RatingSum,
        int ReviewCount,
        string ProfileImageUrl,
        long CountryId,
        List<SpecializationDto> Specializations,
        List<LanguageDto> PreferredLanguages,
        List<SocialMediaDto> SocialMedia
    );

    public record CompanySubscriptionDto(
        long PackageId,
        DateTime StartDate,
        DateTime EndDate,
        string? Coupon,
        double NetPrice
    );

    public record EmployeeDto(
        string Id,
        string FullName,
        string JobTitle,
        string Email,
        string PhoneNumber,
        string GroupName,
        string Country
    );

    public record SpecializationDto(
        long Id,
        string Name
    );

    public record LanguageDto(
        long Id,
        string LanguageName,
        string LanguageCode,
        string CountryName,
        string CountryCode
    );

    public record CreateCompanyEmployeeDto(
        [Required, StringLength(50)] string JobTitle,
        [Range(1, long.MaxValue)] long CountryId,
        [Required, StringLength(50)] string FirstName,
        [Required, StringLength(50)] string LastName,
        [Required, Phone] string PhoneNumber,
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        string Password,
        [Required] string GroupName,
        [Required] string CompanyId
    );

    public record UpdateCompanyDto(
        [Required] string Id,
        [Required, StringLength(100)] string CompanyAddress,
        [Required, StringLength(100)] string CompanyName,
        [Range(1, long.MaxValue)] long CountryId,
        [Required, StringLength(50)] string FirstName,
        [Required, StringLength(50)] string LastName,
        [Required, Phone] string PhoneNumber,
        [Url(ErrorMessage = "Invalid profile image URL.")]
        string ProfileImageUrl
    );

    public record SocialMediaDto(
        long Id,
        string PlatformType,
        string Link
    );

    public record CreateSocialMediaDto(
        [Required] string PlatformType,
        [OptionalUrl(ErrorMessage = "Invalid URL format.")]
        string? Link
    );

    public record ChangeCompanyPasswordDto(
        [Required] string CurrentPassword,
        [Required, RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        string NewPassword,
        [Required]
        string ConfirmPassword
    );
}