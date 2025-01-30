
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
                Status = Common.Enums.ProjectStatus.Pinding,
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
    }

}