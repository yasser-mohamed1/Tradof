
using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Extensions
{
    public static class ProjectExtensions
    {
        public static ProjectDto ToDto(this Data.Entities.Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Days = project.Days,
                Description = project.Description,
                LanguageFromId = project.LanguageFromId,
                LanguageToId = project.LanguageToId,
                MaxPrice = project.MaxPrice,
                MinPrice = project.MinPrice,
                NumberOfOffers = project.Proposals.Count,
                SpecializationId = project.SpecializationId,
                Files = project.Files.Select(f => f.ToDto()).ToList(),
                Price = project.Price,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
            };
        }
        public static Data.Entities.Project ToEntity(this CreateProjectDto projectDto)
        {
            return new Data.Entities.Project
            {
                MinPrice = projectDto.MinPrice,
                Days = projectDto.Days,
                Description = projectDto.Description,
                Name = projectDto.Name,
                MaxPrice = projectDto.MaxPrice,
                Status = Common.Enums.ProjectStatus.Pending,
            };
        }
        public static void UpdateFromDto(this Data.Entities.Project project, UpdateProjectDto projectDto)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            if (projectDto == null) throw new ArgumentNullException(nameof(projectDto));
            project.MinPrice = projectDto.MinPrice;
            project.Days = projectDto.Days;
            project.Description = projectDto.Description;
            project.Name = projectDto.Name;
            project.MaxPrice = projectDto.MaxPrice;

        }

        public static ProjectCardDto ToProjectCardDto(this Data.Entities.Project project)
        {
            string ownerName = project.Company.User.FirstName + " " + project.Company.User.LastName;
            string ownerEmail = project.Company.User.Email ?? "N/A";
            string companyName = project.Company.CompanyName ?? "N/A";
            var budget = new Budget
            {
                MinPrice = project.MinPrice,
                MaxPrice = project.MaxPrice
            };
            return new ProjectCardDto
            {
                ProjectState = project.Status.ToString(),
                ProjectStartDate = project.StartDate,
                Budget = budget,
                Duration = project.Days,
                NumberOfOffers = project.Proposals.Count,
                OwnerName = ownerName,
                OwnerEmail = ownerEmail,
                CompanyName = companyName,
                RegisteredAt = project.Company.CreationDate,
                TotalProjects = project.Company.Projects.Count,
                OpenProjects = project.Company.Projects.Count(p => p.Status != Common.Enums.ProjectStatus.Finished)
            };
        }
    }
}