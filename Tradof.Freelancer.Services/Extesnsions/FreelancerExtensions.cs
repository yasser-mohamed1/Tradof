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
                        RatingSum = acc.RatingSum + rating.RatingValue,
                        ReviewCount = acc.ReviewCount + (string.IsNullOrEmpty(rating.Review) ? 0 : 1)
                    });

            var langauges = freelancer.FreelancerLanguagesPairs.Select(lp => new FreelancerLanguagePairDTO(
                                lp.Id,
                                lp.LanguageFromId,
                                lp.LanguageFrom?.LanguageName ?? "Unknown",
                                lp.LanguageFrom?.LanguageCode ?? "Unknown",
                                lp.LanguageFrom?.CountryCode ?? "Unknown",
                                lp.LanguageFrom?.CountryName ?? "Unknown",
                                lp.LanguageToId,
                                lp.LanguageTo?.LanguageName ?? "Unknown",
                                lp.LanguageTo?.LanguageCode ?? "Unknown",
                                lp.LanguageTo?.CountryCode ?? "Unknown",
                                lp.LanguageTo?.CountryName ?? "Unknown",
                                lp.Free?.HasTakenExam ?? false,
                                lp.Free?.Mark,
                                lp.Pro1?.HasTakenExam ?? false,
                                lp.Pro1?.Mark,
                                lp.Pro2?.HasTakenExam ?? false,
                                lp.Pro2?.Mark
                            )).ToList();

            var socialMedias = freelancer.FreelancerSocialMedias.Select(sm => new FreelancerSocialMediaDTO(
                                   sm.Id,
                                   sm.PlatformType.ToString(),
                                   sm.Link
                               ));

            var specializations = freelancer.Specializations.Select(s => new SpecializationDto(s.Id, s.Name)).ToList();

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
                freelancer.User.ProfileViews,
                langauges,
                socialMedias,
                specializations
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

        public static FreelancerLanguagePairDTO ToFreelancerLanguagePairDTO(this FreelancerLanguagesPair entity)
        {
            return new FreelancerLanguagePairDTO(
                Id: entity.Id,
                LanguageFromId: entity.LanguageFromId,
                LanguageFromName: entity.LanguageFrom?.LanguageName ?? string.Empty,
                LanguageFromCode: entity.LanguageFrom?.LanguageCode ?? string.Empty,
                CountryFromCode: entity.LanguageFrom?.CountryCode ?? string.Empty,
                CountryFromName: entity.LanguageFrom?.CountryName ?? string.Empty,
                LanguageToId: entity.LanguageToId,
                LanguageToName: entity.LanguageTo?.LanguageName ?? string.Empty,
                LanguageToCode: entity.LanguageTo?.LanguageCode ?? string.Empty,
                CountryToCode: entity.LanguageTo?.CountryCode ?? string.Empty,
                CountryToName: entity.LanguageTo?.CountryName ?? string.Empty,
                FreeTaken: entity.Free?.HasTakenExam ?? false,
                FreeMark: entity.Free?.Mark,
                Pro1Taken: entity.Pro1?.HasTakenExam ?? false,
                Pro1Mark: entity.Pro1?.Mark,
                Pro2Taken: entity.Pro2?.HasTakenExam ?? false,
                Pro2Mark: entity.Pro2?.Mark
            );
        }
    }
}