using Tradof.Data.SpecificationParams;
using Tradof.EntityFramework.RequestHelpers;
using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Interfaces
{
    public interface IProjectService
    {
        Task<Pagination<ProjectDto>> GetAllAsync(ProjectSpecParams specParams);
        Task<ProjectDto> GetByIdAsync(long id);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto> UpdateAsync(UpdateProjectDto dto);
        Task<int> GetProjectsCountByMonth(long id, int year, int month);
        Task<bool> DeleteAsync(long id);
        Task<bool> SendReviewRequest(long id);
        Task<bool> MarkAsFinished(long id);

    }
}