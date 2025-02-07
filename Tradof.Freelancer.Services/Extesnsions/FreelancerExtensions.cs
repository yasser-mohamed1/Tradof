using System.ComponentModel.DataAnnotations;
using Tradof.Common.Enums;
using Tradof.Data.Entities;
using Tradof.FreelancerModule.Services.DTOs;

namespace Tradof.FreelancerModule.Services.Extensions
{
    public static class FreelancerExtensions
    {
        public static FreelancerDTO ToFreelancerDTO(this Freelancer freelancer)
        {
            if (freelancer == null) return null;

            var ratingData = freelancer.Projects
                .SelectMany(p => p.Ratings)
                .Aggregate(
                    new { RatingSum = 0.0, ReviewCount = 0 },
                    (acc, rating) => new
                    {
                        RatingSum = acc.RatingSum + (double.TryParse(rating.RatingValue, out var ratingValue) ? ratingValue : 0),
                        ReviewCount = acc.ReviewCount + (string.IsNullOrEmpty(rating.Review) ? 0 : 1)
                    });

            var langauges = freelancer.FreelancerLanguagesPairs.Select(lp => new FreelancerLanguagePairDTO(
                                lp.Id,
                                lp.LanguageFromId,
                                lp.LanguageFrom?.LanguageName ?? "Unknown",
                                lp.LanguageToId,
                                lp.LanguageTo?.LanguageName ?? "Unknown"
                            )).ToList();

            var socialMedias = freelancer.FreelancerSocialMedias.Select(sm => new FreelancerSocialMediaDTO(
                                   sm.Id,
                                   sm.PlatformType.ToString(),
                                   sm.Link
                               ));

            return new FreelancerDTO(
                freelancer.UserId,
                freelancer.User.FirstName,
                freelancer.User.LastName,
                freelancer.User.ProfileImageUrl,
                freelancer.WorkExperience,
                freelancer.Country.Name,
                freelancer.CountryId,
                freelancer.CVFilePath,
                freelancer.User.Email,
                freelancer.User.PhoneNumber,
                ratingData.RatingSum,
                ratingData.ReviewCount,
                langauges,
                socialMedias
            );
        }

        public static void UpdateFromDto(this Freelancer freelancer, UpdateFreelancerDTO dto)
        {
            freelancer.WorkExperience = dto.WorkExperience;
            freelancer.CountryId = dto.CountryId;

            var user = freelancer.User;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.ProfileImageUrl = dto.ProfileImageUrl;
            user.PhoneNumber = dto.PhoneNumber;
        }

        public static FreelancerSocialMedia ToFreelancerSocialMediaEntity(this AddFreelancerSocialMediaDTO dto, long Id)
        {
            if (!Enum.TryParse<PlatformType>(dto.PlatformType, true, out var platformType))
            {
                throw new ArgumentException($"Invalid platform type: {dto.PlatformType}");
            }

            return new FreelancerSocialMedia
            {
                FreelancerId = Id,
                PlatformType = platformType,
                Link = dto.Link,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System"
            };
        }

        public static FreelancerLanguagesPair ToFreelancerLanguagePair(this AddFreelancerLanguagePairDTO dto, long Id) =>
        new()
        {
            LanguageFromId = dto.LanguageFromId,
            LanguageToId = dto.LanguageToId,
            FreelancerId = Id,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        public static PaymentMethod ToPaymentMethodEntity(this PaymentMethodDTO dto) =>
        new()
        {
            CardNumber = dto.CardNumber,
            ExpiryDate = dto.ExpiryDate,
            CVV = dto.CVV,
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
    }
}