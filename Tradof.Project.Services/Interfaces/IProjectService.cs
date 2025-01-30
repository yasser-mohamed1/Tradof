﻿using Tradof.Data.SpecificationParams;
using Tradof.Project.Services.DTOs;
using Tradof.Project.Services.RequestHelpers;

namespace Tradof.Project.Services.Interfaces
{
    public interface IProjectService
    {
        Task<Pagination<ProjectDto>> GetAllAsync(ProjectSpecParams specParams);
        Task<ProjectDto> GetByIdAsync(long id);
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto> UpdateAsync(UpdateProjectDto dto);
        Task<bool> DeleteAsync(long id);
    }
}