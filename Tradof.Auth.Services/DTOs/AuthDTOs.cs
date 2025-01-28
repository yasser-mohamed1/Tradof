using System.ComponentModel.DataAnnotations;

namespace Tradof.Auth.Services.DTOs
{
    public static class ValidationConstants
    {
        public const int MinPasswordLength = 6;
        public const int MaxWorkExperience = 50;
    }

    public record RegisterCompanyDto(
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(ValidationConstants.MinPasswordLength, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        string Password,

        [Required(ErrorMessage = "First name is required.")]
        string FirstName,

        [Required(ErrorMessage = "Last name is required.")]
        string LastName,

        [Required(ErrorMessage = "Company address is required.")]
        string CompanyAddress,

        [Required(ErrorMessage = "Job title is required.")]
        string JobTitle,

        [Required(ErrorMessage = "Country ID is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid Country ID.")]
        long CountryId,

        [Required(ErrorMessage = "Specialization ID is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid Specialization ID.")]
        long SpecializationId,

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        string PhoneNumber,

        string? ProfileImageUrl = null
    );

    public record RegisterFreelancerDto(
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(ValidationConstants.MinPasswordLength, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        string Password,

        [Required(ErrorMessage = "First name is required.")]
        string FirstName,

        [Required(ErrorMessage = "Last name is required.")]
        string LastName,

        [Required(ErrorMessage = "Work experience is required.")]
        [Range(0, ValidationConstants.MaxWorkExperience, ErrorMessage = "Work experience must be between 0 and 50 years.")]
        int WorkExperience,

        [Required(ErrorMessage = "Specialization ID is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid Specialization ID.")]
        long SpecializationId,

        [Required(ErrorMessage = "Country ID is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid Country ID.")]
        long CountryId,

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        string PhoneNumber,

        [Required(ErrorMessage = "Language pairs are required.")]
        IEnumerable<LanguagePairDto> LanguagePairs,

        string? ProfileImageUrl = null
    );

    public record LanguagePairDto(
        [Required(ErrorMessage = "LanguageFromId is required.")]
        long LanguageFromId,

        [Required(ErrorMessage = "LanguageToId is required.")]
        long LanguageToId
    );

    public record LoginDto(
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(ValidationConstants.MinPasswordLength, ErrorMessage = "Password must be at least 6 characters long.")]
        string Password
    );

    public record ForgetPasswordDto(
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email
    );

    public record OtpVerificationDto(
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,

        [Required(ErrorMessage = "OTP is required.")]
        [MinLength(6, ErrorMessage = "OTP must be at least 6 characters.")]
        string Otp
    );

    public record ChangePasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string NewPassword { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Confirm Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}