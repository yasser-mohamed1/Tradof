using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Extensions
{
    public static class StartedProjectExtensions
    {
        public static StartedProjectDto ToStartedDto(this Data.Entities.Project project)
        {
            var specialization = project.Specialization == null
            ? null
            : new SpecializationDto(
                project.SpecializationId ?? 0,
                project.Specialization.Name
            );

            var languageFrom = new LanguageDto(
                project.LanguageFromId,
                project.LanguageFrom.LanguageName,
                project.LanguageFrom.LanguageCode,
                project.LanguageFrom.CountryName,
                project.LanguageFrom.CountryCode
            );

            var languageTo = new LanguageDto(
                project.LanguageToId,
                project.LanguageTo.LanguageName,
                project.LanguageTo.LanguageCode,
                project.LanguageTo.CountryName,
                project.LanguageTo.CountryCode
            );

            var status = new ProjectStatusDto(
                (int)project.Status,
                project.Status.ToString()
            );

            return new StartedProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Days = project.Days,
                Description = project.Description,
                LanguageFrom = languageFrom,
                LanguageTo = languageTo,
                MaxPrice = project.MaxPrice,
                MinPrice = project.MinPrice,
                NumberOfOffers = project.Proposals.Count,
                Specialization = specialization,
                Files = [.. project.Files.Select(f => f.ToDto())],
                Price = project.Price,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = status,
                CompanyId = project.Company.UserId,
                FirstName = project.Company.User.FirstName,
                LastName = project.Company.User.LastName,
                JobTitle = project.Company.JobTitle,
                ProfileImageUrl = project.Freelancer.User.ProfileImageUrl,
                FreelancerFirstName = project.Freelancer.User.FirstName,
                FreelancerLastName = project.Company.User.LastName,
                FreelancerId = project.Freelancer.UserId,
                FreelancerProfileImageUrl = project.Freelancer.User.ProfileImageUrl,
                FreelancerEmail = project.Freelancer.User.Email
            };
        }
    }
}