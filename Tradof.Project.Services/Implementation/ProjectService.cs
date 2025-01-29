using System.ComponentModel.DataAnnotations;
using Tradof.Common.Exceptions;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;
using Tradof.Project.Helpers;
using Tradof.Project.Services.DTOs;
using Tradof.Project.Services.Extensions;
using Tradof.Project.Services.Interfaces;
using Tradof.Project.Services.Validation;
using ProjectEntity = Tradof.Data.Entities.Project;

namespace Tradof.Project.Services.Implementation
{
    public class ProjectService(IUnitOfWork _unitOfWork, IUserHelpers _userHelpers) : IProjectService
    {

        public async Task<IReadOnlyList<ProjectDto>> GetAllAsync()
        {
            var projects = await _unitOfWork.Repository<ProjectEntity>().GetAllAsync();
            return projects.Select(p => p.ToDto()).ToList().AsReadOnly();
        }


        public async Task<ProjectDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid project ID.");
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(id);
            return project == null ? throw new NotFoundException("project not found") : project.ToDto();
        }

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            ValidationHelper.ValidateCreateProjectDto(dto);
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var specialization = await _unitOfWork.Repository<Specialization>().GetByIdAsync(dto.SpecializationId);
            var langFrom = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageFromId);
            var langTo = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageToId);
            List<Data.Entities.File> files = [];
            foreach (var url in dto.Urls)
            {
                var file = await _unitOfWork.Repository<Data.Entities.File>().FindFirstAsync(f => f.FilePath == url);
                files.Add(file);
            }

            var project = dto.ToEntity();
            project.Company.User = currentUser;
            project.LanguageFrom = langFrom;
            project.LanguageTo = langTo;
            project.Specialization = specialization;
            project.Files = files;

            await _unitOfWork.Repository<ProjectEntity>().AddAsync(project);
            if (await _unitOfWork.CommitAsync())
                return project.ToDto();
            else
                throw new Exception("failed to create");

        }

        public async Task<ProjectDto> UpdateAsync(UpdateProjectDto dto)
        {
            ValidationHelper.ValidateUpdateProjectDto(dto);

            var package = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(dto.Id) ?? throw new NotFoundException("Package not found");
            package.UpdateFromDto(dto);

            package.ModificationDate = DateTime.UtcNow;
            package.ModifiedBy = "System";

            await _unitOfWork.Repository<ProjectEntity>().UpdateAsync(package);

            if (await _unitOfWork.CommitAsync())
                return package.ToDto();
            else
                throw new Exception("failed to update");
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid package ID.");
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(id) ?? throw new NotFoundException("project not found");
            await _unitOfWork.Repository<ProjectEntity>().DeleteAsync(project.Id);

            return await _unitOfWork.CommitAsync();

        }
    }
}