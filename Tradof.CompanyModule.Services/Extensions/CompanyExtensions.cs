using System.Runtime.CompilerServices;
using Tradof.Common.Enums;
using Tradof.CompanyModule.Services.DTOs;
using Tradof.Data.Entities;

namespace Tradof.CompanyModule.Services.Extensions
{
    public static class CompanyExtensions
    {
        public static CompanyDto ToDto(this Company company)
        {
            var ratingData = company.Projects
                .SelectMany(p => p.Ratings)
                .Aggregate(
                    new { RatingSum = 0.0, ReviewCount = 0 },
                    (acc, rating) => new
                    {
                        RatingSum = acc.RatingSum + rating.RatingValue,
                        ReviewCount = acc.ReviewCount + (string.IsNullOrEmpty(rating.Review) ? 0 : 1)
                    });

            return new(
                company.UserId,
                company.CompanyAddress,
                company.CompanyName ?? "Unnamed",
                company.User.FirstName,
                company.User.LastName,
                company.User.Email,
                company.User.PhoneNumber,
                company.JobTitle,
                ratingData.RatingSum,
                ratingData.ReviewCount,
                company.User.ProfileImageUrl,
                company.CountryId,
                company.User.ProfileViews,
                company.Specializations.Select(s => new SpecializationDto(s.Id, s.Name)).ToList(),
                company.PreferredLanguages.Select(l => new LanguageDto(l.Id, l.LanguageName, l.LanguageCode, l.CountryName, l.CountryCode)).ToList(),
                company.Medias.Select(m => new SocialMediaDto(m.Id, m.PlatformType.ToString(), m.Link)).ToList()
            );
        }

        public static CompanySubscriptionDto ToDto(this CompanySubscription subscription)
        {
            return new CompanySubscriptionDto(
                subscription.PackageId,
                subscription.StartDate,
                subscription.EndDate,
                subscription.Coupon,
                subscription.NetPrice
            );
        }

        public static CompanyEmployee ToEntity(this CreateCompanyEmployeeDto dto) =>
            new()
            {
                JobTitle = dto.JobTitle,
                CountryId = dto.CountryId,
                GroupName = (GroupName)Enum.Parse(typeof(GroupName), dto.GroupName),
                User = new ApplicationUser
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    PasswordHash = dto.Password
                }
            };

        public static ApplicationUser ToApplicationUser(this CreateCompanyEmployeeDto dto) =>
                new()
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
        public static Language ToLanguage(this LanguageDto dto) =>
               new()
               {
                   LanguageName = dto.LanguageName,
                   LanguageCode = dto.LanguageCode,
                   CountryName = dto.CountryName,
                   CountryCode = dto.CountryCode,
               };

        public static CompanyEmployee ToCompanyEmployee(this CreateCompanyEmployeeDto dto, string userId, Company Company) =>
            new()
            {
                JobTitle = dto.JobTitle,
                CountryId = dto.CountryId,
                UserId = userId,
                GroupName = (GroupName)Enum.Parse(typeof(GroupName), dto.GroupName),
                Company = Company,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = userId,
                ModifiedBy = userId
            };

        public static void UpdateEntity(this UpdateCompanyDto dto, Company company)
        {
            company.CompanyAddress = dto.CompanyAddress;
            company.CountryId = dto.CountryId;
            company.CompanyName = dto.CompanyName;

            if (company.User != null)
            {
                company.User.FirstName = dto.FirstName;
                company.User.LastName = dto.LastName;
                company.User.PhoneNumber = dto.PhoneNumber;
                company.User.ProfileImageUrl = dto.ProfileImageUrl;
            }
        }

        public static CompanySocialMedia ToSocialMedia(this Company company, CreateSocialMediaDto dto)
        {
            if (!Enum.TryParse<PlatformType>(dto.PlatformType, true, out var platformType))
                throw new ArgumentException($"Invalid PlatformType: {dto.PlatformType}");

            return new CompanySocialMedia
            {
                PlatformType = platformType,
                Link = dto.Link,
                CompanyId = company.Id,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = company.UserId,
                ModifiedBy = company.UserId
            };
        }
    }
}
