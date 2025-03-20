using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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

        public async Task<List<StartedProjectDto>> GetStartedProjectsAsync(string companyId)
        {
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == companyId)
                ?? throw new Exception("Company not found.");

            var spec = new StartedProjectsByCompanySpecification(company.Id);
            var items = await _unitOfWork.Repository<ProjectEntity>().GetListWithSpecificationAsync(spec);

            return [.. items.Select(p => p.ToStartedDto())];
        }

        public async Task<List<ProjectDto>> GetInComingProjectsAsync(string companyId)
        {
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == companyId)
                ?? throw new Exception("Company not found.");

            var spec = new PendingProjectsByCompanySpecification(company.Id);
            var items = await _unitOfWork.Repository<ProjectEntity>().GetListWithSpecificationAsync(spec);

            return items.Select(p => p.ToDto()).ToList();
        }

        public async Task<ProjectDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid project ID.");
            var spec = new ProjectSpecification(id);
            var project = await _unitOfWork.Repository<ProjectEntity>().GetEntityWithSpecification(spec);
            return project == null ? throw new NotFoundException("project not found") : project.ToDto();
        }

        public async Task<ProjectDto> CreateAsync(string id, CreateProjectDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Project data cannot be null.");

            ValidationHelper.ValidateCreateProjectDto(dto);
            var currentUser = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(id)
                ?? throw new Exception("Current user not found.");
            var specialization = await _unitOfWork.Repository<Specialization>().GetByIdAsync(dto.SpecializationId);
            var langFrom = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageFromId);
            var langTo = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageToId);
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == id)
                ?? throw new Exception("Company not found.");

            var project = dto.ToEntity();
            project.Company = company;
            project.LanguageFrom = langFrom;
            project.LanguageTo = langTo;
            project.Specialization = specialization;
            project.Status = ProjectStatus.Pending;
            project.CompanyId = company.Id;

            if (dto.Files != null && dto.Files.Any())
            {
                foreach (var file in dto.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLower();

                        if (string.IsNullOrEmpty(fileExtension))
                            throw new Exception("Invalid file type.");

                        if (!Enum.TryParse(typeof(FileType), fileExtension, true, out var fileType))
                            throw new Exception($"Unsupported file type: {fileExtension}");

                        var filePath = Path.Combine("wwwroot/uploads", file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            await file.CopyToAsync(stream);
                        }

                        project.Files.Add(new Data.Entities.File
                        {
                            FilePath = filePath,
                            Project = project,
                            FileName = file.FileName,
                            FileType = (FileType)fileType,
                            ModificationDate = DateTime.UtcNow,
                            CreationDate = DateTime.UtcNow,
                            CreatedBy = currentUser.Id,
                            FileSize = file.Length,
                            ModifiedBy = currentUser.Id
                        });
                    }
                }
            }

            project.CreatedBy = company.User.UserName;
            project.CreationDate = DateTime.UtcNow;
            project.ModifiedBy = company.User.UserName;
            project.ModificationDate = DateTime.UtcNow;

            await _unitOfWork.Repository<ProjectEntity>().AddAsync(project);

            if (await _unitOfWork.CommitAsync())
                return project.ToDto();
            else
                throw new Exception("failed to create");
        }

        public async Task<ProjectDto> UpdateAsync(string id, UpdateProjectDto dto)
        {
            ValidationHelper.ValidateUpdateProjectDto(dto);

            var currentUser = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(id)
                ?? throw new Exception("Current user not found.");
            var specialization = await _unitOfWork.Repository<Specialization>().GetByIdAsync(dto.SpecializationId);
            var langFrom = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageFromId);
            var langTo = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageToId);
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(dto.Id)
                ?? throw new NotFoundException("project not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == id)
                ?? throw new Exception("Company not found.");

            await _unitOfWork.Repository<File>().DeleteWithCrateriaAsync(f => f.ProjectId == project.Id);
            project.Files.Clear();
            await _unitOfWork.CommitAsync();

            project.UpdateFromDto(dto);

            if (dto.Files != null && dto.Files.Any())
            {
                foreach (var file in dto.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLower();

                        if (string.IsNullOrEmpty(fileExtension))
                            throw new Exception("Invalid file type");

                        if (!Enum.TryParse(typeof(FileType), fileExtension, true, out var fileType))
                            throw new Exception($"Unsupported file type: {fileExtension}");

                        var filePath = Path.Combine("wwwroot/uploads", file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        project.Files.Add(new Data.Entities.File
                        {
                            FilePath = filePath,
                            Project = project,
                            FileName = file.FileName,
                            FileType = (FileType)fileType,
                            ModificationDate = DateTime.UtcNow,
                            CreationDate = DateTime.UtcNow,
                            CreatedBy = currentUser.Id,
                            FileSize = file.Length,
                            ModifiedBy = currentUser.Id
                        });
                    }
                }
            }

            project.LanguageFrom = langFrom;
            project.LanguageTo = langTo;
            project.Specialization = specialization;
            project.ModificationDate = DateTime.UtcNow;
            project.ModifiedBy = company.User.UserName;

            await _unitOfWork.Repository<ProjectEntity>().UpdateAsync(project);

            if (await _unitOfWork.CommitAsync())
                return project.ToDto();
            else
                throw new Exception("failed to update");
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid project ID.");
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(id) ?? throw new NotFoundException("project not found");

            if (project.Status != ProjectStatus.Pending)
                throw new Exception("Cannot delete a project after it has started.");

            foreach (var file in project.Files)
            {
                if (System.IO.File.Exists(file.FilePath))
                {
                    System.IO.File.Delete(file.FilePath);
                }
            }
            await _unitOfWork.Repository<ProjectEntity>().DeleteAsync(project.Id);
            await _unitOfWork.Repository<File>().DeleteWithCrateriaAsync(f => f.ProjectId == project.Id);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> GetProjectsCountByMonth(long id, int year, int month)
        {
            return await _unitOfWork.Repository<ProjectEntity>().CountAsync(p => id == p.CompanyId && p.PublishDate.Year == year && p.PublishDate.Month == month);

        }

        public async Task<bool> SendReviewRequest(long id)
        {
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(id) ?? throw new NotFoundException("project not found");
            project.Status = ProjectStatus.OnReviewing;
            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> MarkAsFinished(long id)
        {
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(id) ?? throw new NotFoundException("project not found");
            project.Status = ProjectStatus.Finished;
            return await _unitOfWork.CommitAsync();
        }

        public async Task<Tuple<int, int, int>> ProjectsStatistics(long userId)
        {
            int active = await _unitOfWork.Repository<ProjectEntity>().CountAsync(p => p.FreelancerId == userId && p.Status == ProjectStatus.Active);
            int inProgress = await _unitOfWork.Repository<ProjectEntity>().CountAsync(p => p.FreelancerId == userId && p.Status == ProjectStatus.InProgress);
            int accepted = await _unitOfWork.Repository<ProjectEntity>().CountAsync(p => p.FreelancerId == userId && p.Status == ProjectStatus.Active || p.Status == ProjectStatus.Finished);

            return new Tuple<int, int, int>(active, inProgress, accepted);
        }

        public async Task<ProjectCardDto> GetProjectCardData(long projectId)
        {
            var includes = new List<Expression<Func<ProjectEntity, object>>>
            {
                p => p.Company,
                p => p.Company.User
            };
            var project = await _unitOfWork.Repository<ProjectEntity>().GetByIdAsync(projectId, includes) ?? throw new NotFoundException("project not found");
            var projectCard = project.ToProjectCardDto();

            return projectCard;
        }
    }
}