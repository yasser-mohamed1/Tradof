using Microsoft.AspNetCore.Http;
using Tradof.Data.SpecificationParams;
using Tradof.EntityFramework.RequestHelpers;
using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Interfaces
{
    public interface IProjectService
    {
        Task<Pagination<ProjectDto>> GetAllAsync(ProjectSpecParams specParams);
        Task<Pagination<StartedProjectDto>> GetStartedProjectsAsync(string companyId, int pageIndex, int pageSize);
        Task<List<ProjectDto>> GetInComingProjectsAsync();
        Task<ProjectDto> GetByIdAsync(long id);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto> UpdateAsync(UpdateProjectDto dto);
        Task<int> GetProjectsCountByMonth(int year, int month);
        Task<bool> DeleteAsync(long id);
        Task<bool> SendReviewRequest(long id);
        Task<bool> MarkAsFinished(long id);
        Task<Tuple<int, int, int>> ProjectsStatistics();
        Task<ProjectCardDto> GetProjectCardData(long projectId);
        Task<Pagination<ProjectDto>> GetCurrentProjectsByCompanyIdAsync(string companyId, int pageIndex, int pageSize);
        Task<Pagination<ProjectDto>> GetCurrentProjectsByFreelancerIdAsync(string freelancerId, int pageIndex, int pageSize);
        Task<Pagination<StartedProjectDto>> GetProjectsByFreelancerIdAsync(string freelancerId, int pageIndex, int pageSize);
        Task<Pagination<ProjectDto>> GetUnassignedProjectsAsync(int pageIndex, int pageSize);
        Task<Pagination<ProjectDto>> GetUnassignedProjectsByCompanyAsync(string companyId, int pageIndex, int pageSize);
        Task<List<FileDto>> UploadFilesToProjectAsync(int projectId, List<IFormFile> files);
        Task DeleteFileAsync(int fileId);
    }
}