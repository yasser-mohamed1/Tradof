using System.ComponentModel.DataAnnotations;
using Tradof.Common.Enums;
using Tradof.Common.Exceptions;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;
using Tradof.Data.SpecificationParams;
using Tradof.Data.Specifications;
using Tradof.EntityFramework.Helpers;
using Tradof.EntityFramework.RequestHelpers;
using Tradof.Project.Services.DTOs;
using Tradof.Project.Services.Extensions;
using Tradof.Project.Services.Interfaces;
using Tradof.Project.Services.Validation;
using File = Tradof.Data.Entities.File;
using ProjectEntity = Tradof.Data.Entities.Project;

namespace Tradof.Project.Services.Implementation
{
    public class ProjectService(IUnitOfWork _unitOfWork, IUserHelpers _userHelpers) : IProjectService
    {

        public async Task<Pagination<ProjectDto>> GetAllAsync(ProjectSpecParams specParams)
        {
            var specification = new ProjectFilterSortPaginationSpecification(specParams);
            var items = await _unitOfWork.Repository<ProjectEntity>().ListAsync(specification);
            var count = await _unitOfWork.Repository<ProjectEntity>().CountAsync(specification);
            var dtos = items.Select(p => p.ToDto()).ToList();
            var pagination = new Pagination<ProjectDto>(specParams.PageIndex, specParams.PageSize, count, dtos);

            return pagination;
        }


        public async Task<ProjectDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid project ID.");
            var spec = new ProjectSpecification(id);
            var project = await _unitOfWork.Repository<ProjectEntity>().GetEntityWithSpecification(spec);
            return project == null ? throw new NotFoundException("project not found") : project.ToDto();
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Project data cannot be null.");

            ValidationHelper.ValidateCreateProjectDto(dto);
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var specialization = await _unitOfWork.Repository<Specialization>().GetByIdAsync(dto.SpecializationId);
            var langFrom = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageFromId);
            var langTo = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageToId);
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == currentUser.Id) ?? throw new Exception("current user not found");

            var project = dto.ToEntity();
            project.Company = company;
            project.LanguageFrom = langFrom;
            project.LanguageTo = langTo;
            project.Specialization = specialization;
            foreach (var url in dto.Urls)
            {
                project.Files.Add(new Data.Entities.File
                {
                    FilePath = url,
                    Project = project,
                    FileName = url,
                    FileType = FileType.Excel,
                    ModificationDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    CreatedBy = currentUser.Email,
                    FileSize = 4,
                    ModifiedBy = currentUser.Email
                });
            }
            project.CreatedBy = company.User.UserName;
            project.CreationDate = DateTime.Now;
            project.ModifiedBy = company.User.UserName;
            project.ModificationDate = DateTime.Now;
            await _unitOfWork.Repository<ProjectEntity>().AddAsync(project);
            if (await _unitOfWork.CommitAsync())
                return project.ToDto();
            else
                throw new Exception("failed to create");

        }

        public async Task<ProjectDto> UpdateAsync(UpdateProjectDto dto)
        {
            ValidationHelper.ValidateUpdateProjectDto(dto);
            var specialization = await _unitOfWork.Repository<Specialization>().GetByIdAsync(dto.SpecializationId);
            var langFrom = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageFromId);
            var langTo = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageToId);
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(dto.Id) ?? throw new NotFoundException("Package not found");

            await _unitOfWork.Repository<File>().DeleteWithCrateriaAsync(f => f.ProjectId == project.Id);
            project.Files.Clear();
            await _unitOfWork.CommitAsync();

            project.UpdateFromDto(dto);

            foreach (var url in dto.Urls)
            {
                project.Files.Add(new Data.Entities.File
                {
                    FilePath = url,
                    Project = project,
                    FileName = url,
                    FileType = FileType.Excel,
                    ModificationDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    CreatedBy = "system",
                    FileSize = 4,
                    ModifiedBy = "system"
                });
            }
            project.LanguageFrom = langFrom;
            project.LanguageTo = langTo;
            project.Specialization = specialization;
            project.ModificationDate = DateTime.UtcNow;
            project.ModifiedBy = "System";

            await _unitOfWork.Repository<ProjectEntity>().UpdateAsync(project);

            if (await _unitOfWork.CommitAsync())
                return project.ToDto();
            else
                throw new Exception("failed to update");
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid package ID.");
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(id) ?? throw new NotFoundException("project not found");

            await _unitOfWork.Repository<ProjectEntity>().DeleteAsync(project.Id);
            await _unitOfWork.Repository<File>().DeleteWithCrateriaAsync(f => f.ProjectId == project.Id);
            return await _unitOfWork.CommitAsync();

        }
    }
}