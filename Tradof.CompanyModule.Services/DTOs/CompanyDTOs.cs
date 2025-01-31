using System.ComponentModel.DataAnnotations;
using Tradof.Common.Enums;
using Tradof.Data.Entities;

namespace Tradof.CompanyModule.Services.DTOs
{
    public record CompanyDto(
        long Id,
        string CompanyAddress,
        string UserId,
        long CountryId,
        List<string> Specializations,
        List<string> PreferredLanguages,
        List<SocialMediaDto> SocialMedia,
        double NetPrice,
        DateTime SubscriptionStartDate,
        DateTime SubscriptionEndDate
    );

    public record CreateCompanyEmployeeDto(
        [Required, StringLength(50)] string JobTitle,
        [Range(1, long.MaxValue)] long CountryId,
        [Required, StringLength(50)] string FirstName,
        [Required, StringLength(50)] string LastName,
        [Required, Phone] string PhoneNumber,
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)] string Password,
        [Required] string GroupName,
        [Required] string CompanyId
    );

    public record UpdateCompanyDto(
        [Required] string Id,
        [Required, StringLength(100)] string CompanyAddress,
        [Range(1, long.MaxValue)] long CountryId,
        [Required, StringLength(50)] string FirstName,
        [Required, StringLength(50)] string LastName,
        [Required, Phone] string PhoneNumber,
        [Required, EmailAddress] string Email
    );

    public record SocialMediaDto(
        [Required] string PlatformType,
        [Required, Url] string Link
    );

    public record ChangeCompanyPasswordDto(
        [Required] string CompanyId,
        [Required] string CurrentPassword,
        [Required, MinLength(8)] string NewPassword
    );
}