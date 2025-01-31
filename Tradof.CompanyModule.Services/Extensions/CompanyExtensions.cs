using Tradof.Common.Enums;
using Tradof.CompanyModule.Services.DTOs;
using Tradof.Data.Entities;

namespace Tradof.CompanyModule.Services.Extensions
{
    public static class CompanyExtensions
    {
        public static CompanyDto ToDto(this Company company) =>
            new(
                company.UserId,
                company.CompanyAddress,
                company.CompanyName ?? "Unnamed",
                company.CountryId,
                company.Specializations.Select(s => new SpecializationDto(s.Id, s.Name)).ToList(),
                company.PreferredLanguages.Select(l => new LanguageDto(l.Id, l.Name, l.Code)).ToList(),
                company.Medias.Select(m => new SocialMediaDto(m.PlatformType.ToString(), m.Link)).ToList(),
                company.Subscriptions.FirstOrDefault()?.NetPrice ?? 0,
                company.Subscriptions.FirstOrDefault()?.StartDate ?? DateTime.MinValue,
                company.Subscriptions.FirstOrDefault()?.EndDate ?? DateTime.MinValue
            );

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
                CreatedBy = "System",
                ModifiedBy = "System"
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
                company.User.Email = dto.Email;
            }
        }
    }
}
