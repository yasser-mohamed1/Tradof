using System.ComponentModel.DataAnnotations;

namespace Tradof.FreelancerModule.Services.DTOs
{
    public record FreelancerDTO(
        string UserId,
        string FirstName,
        string LastName,
        string ProfileImageUrl,
        int WorkExperience,
        string CountryName,
        long CountryId,
        string CVFilePath,
        string Email,
        string Phone,
        double RatingSum,
        int ReviewCount,
        IEnumerable<FreelancerLanguagePairDTO> FreelancerLanguagePairs,
        IEnumerable<FreelancerSocialMediaDTO> FreelancerSocialMedias,
        IEnumerable<SpecializationDto> FreelancerSpecializations
    );

    public record UpdateFreelancerDTO(
        [Required(ErrorMessage = "Work experience is required.")]
        int WorkExperience,

        [Required(ErrorMessage = "Country ID is required.")]
        long CountryId,

        [Required(ErrorMessage = "First name is required.")]
        string FirstName,

        [Required(ErrorMessage = "Last name is required.")]
        string LastName,

        [Url(ErrorMessage = "Invalid profile image URL.")]
        string ProfileImageUrl,

        [Phone(ErrorMessage = "Invalid phone number format.")]
        string PhoneNumber
    );

    public record ChangePasswordDTO(
        [Required] string CurrentPassword,
        [Required, RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must be at least 6 characters long, and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        string NewPassword,
        [Required]
        string ConfirmPassword
    );

    public record PaymentMethodDTO(
        [Required, CreditCard(ErrorMessage = "Invalid credit card number.")]
        string CardNumber,

        [Required, RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiry date must be in MM/YY format.")]
        string ExpiryDate,

        [Required, Range(100, 9999, ErrorMessage = "CVV must be between 3 and 4 digits.")]
        int CVV
    );

    public record AddFreelancerSocialMediaDTO(
        [Required(ErrorMessage = "Platform type is required.")]
        string PlatformType,

        [OptionalUrl(ErrorMessage = "Invalid URL format.")]
        string Link
    );

    public record FreelancerSocialMediaDTO(
        long Id,
        string PlatformType,
        string Link
    );

    public record UpdateFreelancerSocialMediaDTO(
        [Required(ErrorMessage = "Link is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        string Link
    );

    public record FreelancerLanguagePairDTO(
        long Id,
        long LanguageFromId,
        string LanguageFromName,
        string LanguageFromCode,
        string CountryFromCode,
        string CountryFromName,
        long LanguageToId,
        string LanguageToName,
        string LanguageToCode,
        string CountryToCode,
        string CountryToName
    );

    public record AddFreelancerLanguagePairDTO(
        [Required(ErrorMessage = "LanguageFromId is required.")]
        long LanguageFromId,

        [Required(ErrorMessage = "LanguageToId is required.")]
        long LanguageToId
    );

    public record RemoveFreelancerLanguagePairDTO(
        [Required(ErrorMessage = "LanguageFromId is required.")]
        long LanguageFromId,

        [Required(ErrorMessage = "LanguageToId is required.")]
        long LanguageToId
    );

    public record SpecializationDto(
        long Id,
        string Name
    );
}