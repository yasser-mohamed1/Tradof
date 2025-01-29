using Tradof.Auth.Services.DTOs;
using Tradof.Common.Enums;
using Tradof.Data.Entities;

namespace Tradof.Auth.Services.Extensions
{
    public static class DtoExtensions
    {
        public static Company ToCompanyEntity(this RegisterCompanyDto dto, ApplicationUser newUser)
        {
            return new Company
            {
                CompanyAddress = dto.CompanyAddress,
                JobTitle = dto.JobTitle,
                CountryId = dto.CountryId,
                GroupName = GroupName.Adminstrator,
                UserId = newUser.Id,
                User = newUser,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System"
            };
        }

        public static ApplicationUser ToApplicationUserEntity(this RegisterCompanyDto dto)
        {
            return new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserType = UserType.CompanyAdmin,
                ProfileImageUrl = dto.ProfileImageUrl,
                PhoneNumber = dto.PhoneNumber,
                IsEmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };
        }

        public static Freelancer ToFreelancerEntity(this RegisterFreelancerDto dto, ApplicationUser newUser)
        {
            return new Freelancer
            {
                WorkExperience = dto.WorkExperience,
                CountryId = dto.CountryId,
                SpecializationId = dto.SpecializationId,
                UserId = newUser.Id,
                User = newUser,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System"
            };
        }

        public static ApplicationUser ToApplicationUserEntity(this RegisterFreelancerDto dto)
        {
            return new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserType = UserType.Freelancer,
                ProfileImageUrl = dto.ProfileImageUrl,
                PhoneNumber = dto.PhoneNumber,
                IsEmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };
        }

        public static FreelancerLanguagesPair ToFreelancerLanguagesPairEntity(this LanguagePairDto dto, Freelancer newFreelancer)
        {
            return new FreelancerLanguagesPair
            {
                FreelancerId = newFreelancer.Id,
                LanguageFromId = dto.LanguageFromId,
                LanguageToId = dto.LanguageToId,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System"
            };
        }
    }
}