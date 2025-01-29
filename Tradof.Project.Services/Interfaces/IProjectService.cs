using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IReadOnlyList<ProjectDto>> GetAllAsync();
        Task<ProjectDto> GetByIdAsync(long id);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto> UpdateAsync(UpdateProjectDto dto);
        Task<bool> DeleteAsync(long id);
    }
}