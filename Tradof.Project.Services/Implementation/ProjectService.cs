using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Expressions;
using System.Net.Http.Json;
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
using Tradof.ResponseHandler.Models;
using File = Tradof.Data.Entities.File;
using ProjectEntity = Tradof.Data.Entities.Project;

namespace Tradof.Project.Services.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserHelpers _userHelpers;
        private readonly INotificationService _notificationService;
        private readonly Cloudinary _cloudinary;

        public ProjectService(
            IUnitOfWork unitOfWork,
            IUserHelpers userHelpers,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userHelpers = userHelpers;
            _notificationService = notificationService;
            _cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
        }

        public async Task<Pagination<ProjectDto>> GetAllAsync(ProjectSpecParams specParams)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id);
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id);

            if (specParams.CompanyId != null && company != null)
            {
                if (company.UserId != specParams.CompanyId)
                    throw new Exception("not authorized");
            }
            if (specParams.FreelancerId != null && freelancer != null)
            {
                if (freelancer.UserId != specParams.FreelancerId)
                    throw new Exception("not authorized");
            }
            var specification = new ProjectFilterSortPaginationSpecification(specParams);
            var items = await _unitOfWork.Repository<ProjectEntity>().ListAsync(specification);
            var count = await _unitOfWork.Repository<ProjectEntity>().CountAsync(specification);
            var dtos = items.Select(p => p.ToDto()).ToList();
            var pagination = new Pagination<ProjectDto>(specParams.PageIndex, specParams.PageSize, count, dtos);

            return pagination;
        }

        public async Task<Pagination<StartedProjectDto>> GetStartedProjectsAsync(string companyId, int pageIndex, int pageSize)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");

            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == companyId)
                ?? throw new Exception("Company not found.");

            var spec = new StartedProjectsByCompanySpecification(company.Id, pageIndex, pageSize);
            var items = await _unitOfWork.Repository<ProjectEntity>().GetListWithSpecificationAsync(spec);
            var count = await _unitOfWork.Repository<ProjectEntity>().CountAsync(new StartedProjectsByCompanySpecification(company.Id));

            var dtos = items.Select(p => p.ToStartedDto()).ToList();
            return new Pagination<StartedProjectDto>(pageIndex, pageSize, count, dtos);
        }

        public async Task<List<ProjectDto>> GetInComingProjectsAsync()
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == currentUser.Id)
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

        public async Task<ProjectDto> CreateAsync(CreateProjectDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Project data cannot be null.");

            ValidationHelper.ValidateCreateProjectDto(dto);
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == currentUser.Id)
                ?? throw new Exception("Company not found.");
            var specialization = await _unitOfWork.Repository<Specialization>().GetByIdAsync(dto.SpecializationId);
            var langFrom = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageFromId);
            var langTo = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageToId);

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

                        var (cloudinaryUrl, publicId) = await UploadToCloudinaryAsync(file);

                        project.Files.Add(new Data.Entities.File
                        {
                            FilePath = cloudinaryUrl,
                            Project = project,
                            FileName = file.FileName,
                            FileType = (FileType)fileType,
                            ModificationDate = DateTime.UtcNow,
                            CreationDate = DateTime.UtcNow,
                            CreatedBy = currentUser.Id,
                            FileSize = file.Length,
                            ModifiedBy = currentUser.Id,
                            PublicId = publicId
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

        public async Task<ProjectDto> UpdateAsync(UpdateProjectDto dto)
        {
            ValidationHelper.ValidateUpdateProjectDto(dto);

            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == currentUser.Id)
                ?? throw new Exception("Company not found.");
            var specialization = await _unitOfWork.Repository<Specialization>().GetByIdAsync(dto.SpecializationId);
            var langFrom = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageFromId);
            var langTo = await _unitOfWork.Repository<Language>().GetByIdAsync(dto.LanguageToId);
            var project = await _unitOfWork.Repository<ProjectEntity>().FindFirstAsync(p => p.Id == dto.Id && p.CompanyId == company.Id)
                ?? throw new NotFoundException("project not found");

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

                        var (cloudinaryUrl, publicId) = await UploadToCloudinaryAsync(file);

                        project.Files.Add(new Data.Entities.File
                        {
                            FilePath = cloudinaryUrl,
                            Project = project,
                            FileName = file.FileName,
                            FileType = (FileType)fileType,
                            ModificationDate = DateTime.UtcNow,
                            CreationDate = DateTime.UtcNow,
                            CreatedBy = currentUser.Id,
                            FileSize = file.Length,
                            ModifiedBy = currentUser.Id,
                            PublicId = publicId
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
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Validate input
                if (id <= 0)
                {
                    throw new ValidationException("Invalid project ID. ID must be greater than 0.");
                }

                // Get current user with proper error handling
                var currentUser = await _userHelpers.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    throw new UnauthorizedAccessException("User authentication failed. Please log in again.");
                }

                // Get company with proper error handling
                var company = await _unitOfWork.Repository<Company>()
                    .FindFirstAsync(c => c.UserId == currentUser.Id);
                if (company == null)
                {
                    throw new NotFoundException("Company profile not found for the current user.");
                }

                // Get project with eager loading of files and related data
                var project = await _unitOfWork.Repository<ProjectEntity>()
                    .FindFirstAsync(
                        p => p.Id == id && p.CompanyId == company.Id,
                        includes: new List<Expression<Func<ProjectEntity, object>>>
                                  {
                                      p =>  p.Files,
                                      p => p.Proposals,
                                      p => p.Ratings
                                  }
                    );

                if (project == null)
                {
                    throw new NotFoundException($"Project with ID {id} not found or you don't have permission to delete it.");
                }

                // Validate project status - more comprehensive check
                if (!CanDeleteProject(project))
                {
                    throw new InvalidOperationException(
                        $"Cannot delete project. Current status: {project.Status}. " +
                        "Only projects with 'Pending' status can be deleted."
                    );
                }

                // Check for active proposals that might prevent deletion
                var activeProposals = project.Proposals?.Where(p => p.ProposalStatus == ProposalStatus.Pending).ToList();
                if (activeProposals?.Any() == true)
                {
                    // Log warning but continue - you might want to handle this differently
                    // Consider notifying freelancers or rejecting proposals first
                }

                // Handle file deletion with comprehensive error handling
                var fileDeleteTasks = new List<Task>();

                if (project.Files?.Any() == true)
                {
                    foreach (var file in project.Files)
                    {
                        fileDeleteTasks.Add(DeleteFileAsync(file.Id));
                    }

                    // Wait for all file deletions to complete
                    await Task.WhenAll(fileDeleteTasks);
                }

                // Delete related entities in correct order to avoid foreign key violations

                // 1. Delete file records from database
                if (project.Files?.Any() == true)
                {
                    var fileIds = project.Files.Select(f => f.Id).ToList();
                    await _unitOfWork.Repository<File>()
                        .DeleteWithCrateriaAsync(f => fileIds.Contains(f.Id));
                }

                // 2. Delete ratings
                if (project.Ratings?.Any() == true)
                {
                    await _unitOfWork.Repository<Rating>()
                        .DeleteWithCrateriaAsync(r => r.ProjectId == project.Id);
                }

                // 3. Delete or update proposals (depending on your business logic)
                if (project.Proposals?.Any() == true)
                {
                    // Option 1: Delete proposals
                    await _unitOfWork.Repository<Proposal>()
                        .DeleteWithCrateriaAsync(p => p.ProjectId == project.Id);

                    // Option 2: Mark proposals as cancelled (alternative approach)
                    // foreach (var proposal in project.Proposals)
                    // {
                    //     proposal.Status = ProposalStatus.Cancelled;
                    //     await _unitOfWork.Repository<Proposal>().UpdateAsync(proposal);
                    // }
                }

                // 4. Finally delete the project
                await _unitOfWork.Repository<ProjectEntity>().DeleteAsync(project.Id);

                // Commit transaction
                var result = await _unitOfWork.CommitAsync();

                if (result)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("Failed to delete project. Database operation unsuccessful.");
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Log the exception with context
                // _logger.LogError(ex, "Failed to delete project {ProjectId} for user {UserId}", id, currentUser?.Id);

                // Re-throw with additional context if it's not already a known exception type
                if (ex is ValidationException or NotFoundException or UnauthorizedAccessException or InvalidOperationException)
                {
                    throw;
                }

                throw new InvalidOperationException($"An error occurred while deleting the project: {ex.Message}", ex);
            }
        }

        private static bool CanDeleteProject(ProjectEntity project)
        {
            // Define which statuses allow deletion
            var deletableStatuses = new[]
            {
                ProjectStatus.Pending,
            };

            // Additional business rules
            if (project.CancellationRequested && !project.CancellationAccepted)
            {
                return false; // Don't allow deletion if cancellation is pending
            }

            if (project.FreelancerId.HasValue && project.Status != ProjectStatus.Pending)
            {
                return false; // Don't allow deletion if freelancer is assigned and project has started
            }

            return deletableStatuses.Contains(project.Status);
        }

        public async Task<List<ProjectGroupResult>> GetProjectsCountAndCostAsync(int? year, int? month)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("User not found.");
            var company = await _unitOfWork.Repository<Company>()
                .FindFirstAsync(c => c.UserId == currentUser.Id) ?? throw new Exception("Company not found.");

            var query = _unitOfWork.Repository<ProjectEntity>()
                .GetQueryable()
                .Where(p => p.CompanyId == company.Id);

            DateTime fromDate;
            DateTime toDate = DateTime.Now;

            if (year.HasValue)
            {
                fromDate = new DateTime(year.Value, 1, 1);
                toDate = new DateTime(year.Value, 12, 31, 23, 59, 59);
            }
            else
            {
                fromDate = DateTime.Now.AddMonths(-12);
            }

            if (!year.HasValue && !month.HasValue)
            {
                query = query.Where(p => p.PublishDate >= fromDate && p.PublishDate <= toDate);
            }
            if (year.HasValue)
            {
                query = query.Where(p => p.PublishDate.Year == year.Value);
            }
            if (month.HasValue)
            {
                query = query.Where(p => p.PublishDate.Month == month.Value);
            }

            if (month.HasValue)
            {
                query = query.Where(p => p.PublishDate.Month == month.Value);

                return await query
                    .GroupBy(p => p.PublishDate.Day)
                    .OrderBy(g => g.Key)
                    .Select(g => new ProjectGroupResult
                    {
                        Key = g.Key,
                        Count = g.Count(),
                        TotalCost = g.Sum(p => p.Price)
                    })
                    .ToListAsync();
            }
            else
            {
                return await query
                    .GroupBy(p => p.PublishDate.Month)
                    .OrderBy(g => g.Key)
                    .Select(g => new ProjectGroupResult
                    {
                        Key = g.Key,
                        Count = g.Count(),
                        TotalCost = g.Sum(p => p.Price)
                    })
                    .ToListAsync();
            }
        }

        public async Task<bool> SendReviewRequest(long projectId, string freelancerId)
        {
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(c => c.UserId == freelancerId, [f => f.User])
                ?? throw new Exception("freelancer not found.");
            var project = await _unitOfWork.Repository<ProjectEntity>().FindFirstAsync(p => p.Id == projectId && p.FreelancerId == freelancer.Id, [p => p.Company, p => p.Company.User]) ?? throw new NotFoundException("project not found");
            project.Status = ProjectStatus.OnReviewing;

            // Send notification to company
            var notification = new NotificationDto
            {
                type = "Project",
                senderId = project.Freelancer.User.Id,
                receiverId = project.Company.User.Id,
                message = $"Review request sent for project '{project.Name}'.",
                description = $"Please review the project '{project.Name}' and provide your feedback.",
                timestamp = DateTime.UtcNow
            };
            await _notificationService.SendNotificationAsync(notification);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> MarkAsFinished(long id)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == currentUser.Id)
                ?? throw new Exception("Company not found.");
            var project = await _unitOfWork.Repository<ProjectEntity>().FindFirstAsync(p => p.Id == id && p.CompanyId == company.Id,
                 includes: [p => p.Freelancer, p => p.Freelancer.User, p => p.Company, p => p.Company.User]) ?? throw new NotFoundException("project not found");

            // Check payment status
            using (var httpClient = new HttpClient())
            {
                var paymentStatusResponse = await httpClient.GetAsync($"https://tradofapi-production.up.railway.app/api/financial/payment-status/{id}");
                if (!paymentStatusResponse.IsSuccessStatusCode)
                    throw new Exception("Failed to check payment status");

                var paymentStatus = await paymentStatusResponse.Content.ReadFromJsonAsync<PaymentStatusResponse>() ?? throw new Exception("Invalid payment status response");
                if (paymentStatus.PaymentStatus.ToLower() == "pending")
                    throw new Exception("Cannot mark project as finished: Payment is pending");

                // If payment is not pending, proceed with finishing the project
                var finishProjectResponse = await httpClient.PostAsync($"https://tradofapi-production.up.railway.app/api/payment/finish-project/{id}", null);
                if (!finishProjectResponse.IsSuccessStatusCode)
                    throw new Exception("Failed to process project completion");

                var finishResult = await finishProjectResponse.Content.ReadFromJsonAsync<FinishProjectResponse>();
                if (finishResult == null || !finishResult.Success)
                    throw new Exception(finishResult?.Message ?? "Failed to complete project");
            }

            project.Status = ProjectStatus.Finished;
            project.DeliveryDate = DateTime.UtcNow;
            var result = await _unitOfWork.CommitAsync();

            if (result && project.Freelancer != null)
            {
                // Send notification to freelancer
                var notification = new NotificationDto
                {
                    type = "Project",
                    senderId = project.Freelancer.User.Id,
                    receiverId = project.Company.User.Id,
                    message = $"The project '{project.Name}' is finished successfully",
                    description = $"Project '{project.Name}' has been marked as completed",
                    timestamp = DateTime.UtcNow
                };
                await _notificationService.SendNotificationAsync(notification);
            }

            return result;
        }

        public async Task<bool> RequestProjectCancellation(long projectId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == currentUser.Id, includes: [p => p.User])
                ?? throw new Exception("Company not found.");

            var project = await _unitOfWork.Repository<ProjectEntity>()
                .FindFirstAsync(p => p.Id == projectId && p.CompanyId == company.Id,
                    includes: [p => p.Freelancer, p => p.Freelancer.User])
                ?? throw new NotFoundException("project not found");

            if (project.Status != ProjectStatus.Active && project.Status != ProjectStatus.InProgress)
                throw new Exception("Can only request cancellation for active or in-progress projects");

            if (project.CancellationRequested)
                throw new Exception("Cancellation has already been requested for this project");

            project.CancellationRequested = true;
            project.CancellationRequestedBy = currentUser.Id;
            project.CancellationRequestDate = DateTime.UtcNow;

            var result = await _unitOfWork.CommitAsync();

            if (result && project.Freelancer != null)
            {
                // Send notification to the freelancer
                var notification = new NotificationDto
                {
                    type = "Project",
                    senderId = project.Company.User.Id,
                    receiverId = project.Freelancer.User.Id,
                    message = $"Project '{project.Name}' cancellation has been requested.",
                    description = $"A cancellation request has been submitted for project '{project.Name}'.",
                    timestamp = DateTime.UtcNow
                };
                await _notificationService.SendNotificationAsync(notification);
            }

            return result;
        }

        public async Task<bool> AcceptProjectCancellation(long projectId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id, includes: [p => p.User])
                ?? throw new Exception("Freelancer not found");

            var project = await _unitOfWork.Repository<ProjectEntity>()
                .FindFirstAsync(p => p.Id == projectId && p.FreelancerId == freelancer.Id, includes: [p => p.Company, p => p.Company.User])
                ?? throw new NotFoundException("project not found");

            if (!project.CancellationRequested)
                throw new Exception("No cancellation request exists for this project");

            if (project.Status != ProjectStatus.Active && project.Status != ProjectStatus.InProgress)
                throw new Exception("Can only accept cancellation for active or in-progress projects");

            project.Status = ProjectStatus.Cancelled;
            project.CancellationAccepted = true;
            project.CancellationAcceptedDate = DateTime.UtcNow;
            project.CancellationAcceptedBy = currentUser.Id;
            project.CancellationResponse = "Accepted";

            var result = await _unitOfWork.CommitAsync();

            if (result && project.Company != null)
            {
                // Send notification to the company
                var notification = new NotificationDto
                {
                    type = "Project",
                    senderId = project.Company.User.Id,
                    receiverId = project.Freelancer.User.Id,
                    message = $"Your project cancellation request for '{project.Name}' has been accepted.",
                    description = $"The company has accepted your cancellation request for project '{project.Name}'.",
                    timestamp = DateTime.UtcNow
                };
                await _notificationService.SendNotificationAsync(notification);
            }

            return result;
        }

        public async Task<bool> RejectProjectCancellation(long projectId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id,
                includes: [p => p.User])
                ?? throw new Exception("Freelancer not found");

            var project = await _unitOfWork.Repository<ProjectEntity>()
                .FindFirstAsync(p => p.Id == projectId && p.FreelancerId == freelancer.Id,
                    includes: [p => p.Company, p => p.Company.User])
                ?? throw new NotFoundException("project not found");

            if (!project.CancellationRequested)
                throw new Exception("No cancellation request exists for this project");

            project.CancellationRequested = false;
            project.CancellationRequestedBy = null;
            project.CancellationRequestDate = null;
            project.CancellationResponse = "Rejected";

            var result = await _unitOfWork.CommitAsync();

            if (result && project.Company != null)
            {
                // Send notification to the company
                var notification = new NotificationDto
                {
                    type = "Project",
                    senderId = project.Company.User.Id,
                    receiverId = project.Freelancer.User.Id,
                    message = $"Your project cancellation request for '{project.Name}' has been rejected.",
                    description = $"The company has rejected your cancellation request for project '{project.Name}'.",
                    timestamp = DateTime.UtcNow
                };
                await _notificationService.SendNotificationAsync(notification);
            }

            return result;
        }

        public async Task<Tuple<int, int, int>> ProjectsStatistics()
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("user not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(c => c.UserId == currentUser.Id)
                ?? throw new Exception("freelancer not found.");

            int active = await _unitOfWork.Repository<ProjectEntity>().CountAsync(p =>
                    p.FreelancerId == freelancer.Id &&
                    (p.Status == ProjectStatus.Active || p.Status == ProjectStatus.InProgress || p.Status == ProjectStatus.OnReviewing));

            int completed = await _unitOfWork.Repository<ProjectEntity>().CountAsync(p =>
                p.FreelancerId == freelancer.Id && p.Status == ProjectStatus.Finished);

            int cancelled = await _unitOfWork.Repository<ProjectEntity>().CountAsync(p =>
                p.FreelancerId == freelancer.Id && p.Status == ProjectStatus.Cancelled);

            return new Tuple<int, int, int>(active, completed, cancelled);
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

        private async Task<(string Url, string PublicId)> UploadToCloudinaryAsync(IFormFile file)
        {
            string publicId = $"projects/{Guid.NewGuid()}_{file.FileName}";

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = publicId,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return (uploadResult.SecureUrl.AbsoluteUri, uploadResult.PublicId);
            }
        }

        public async Task<Pagination<ProjectDto>> GetCurrentProjectsByCompanyIdAsync(string companyId, int pageIndex, int pageSize)
        {
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(c => c.UserId == companyId)
                ?? throw new Exception("Company not found.");

            var spec = new CurrentProjectsByCompanySpecification(company.Id, pageIndex, pageSize);
            var projects = await _unitOfWork.Repository<ProjectEntity>().ListAsync(spec);
            var totalCount = await _unitOfWork.Repository<ProjectEntity>().CountAsync(new CurrentProjectsByCompanySpecification(company.Id));

            var dtos = projects.Select(p => p.ToDto()).ToList();
            return new Pagination<ProjectDto>(pageIndex, pageSize, totalCount, dtos);
        }

        public async Task<Pagination<ProjectDto>> GetCurrentProjectsByFreelancerIdAsync(string freelancerId, int pageIndex, int pageSize)
        {
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == freelancerId)
                ?? throw new Exception("Freelancer not found.");

            var spec = new CurrentProjectsByFreelancerSpecification(freelancer.Id, pageIndex, pageSize);
            var projects = await _unitOfWork.Repository<ProjectEntity>().ListAsync(spec);
            var totalCount = await _unitOfWork.Repository<ProjectEntity>().CountAsync(new CurrentProjectsByFreelancerSpecification(freelancer.Id));

            var dtos = projects.Select(p => p.ToDto()).ToList();
            return new Pagination<ProjectDto>(pageIndex, pageSize, totalCount, dtos);
        }

        public async Task<Pagination<StartedProjectDto>> GetProjectsByFreelancerIdAsync(string freelancerId, int pageIndex, int pageSize)
        {
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == freelancerId)
                ?? throw new Exception("Freelancer not found.");

            var spec = new ProjectsByFreelancerSpecification(freelancer.Id, pageIndex, pageSize);
            var projects = await _unitOfWork.Repository<ProjectEntity>().ListAsync(spec);
            var totalCount = await _unitOfWork.Repository<ProjectEntity>().CountAsync(new ProjectsByFreelancerSpecification(freelancer.Id));

            var dtos = projects.Select(p => p.ToStartedDto()).ToList();
            return new Pagination<StartedProjectDto>(pageIndex, pageSize, totalCount, dtos);
        }

        public async Task<Pagination<ProjectDto>> GetUnassignedProjectsAsync(UnassignedProjectsSpecParams specParams)
        {
            var spec = new UnassignedProjectsSpecification(specParams);
            var projects = await _unitOfWork.Repository<ProjectEntity>().ListAsync(spec);

            var totalCount = await _unitOfWork.Repository<ProjectEntity>().CountAsync(spec);

            var dtos = projects.Select(p => p.ToDto()).ToList();
            return new Pagination<ProjectDto>(specParams.PageIndex, specParams.PageSize, totalCount, dtos);
        }

        public async Task<Pagination<ProjectDto>> GetUnassignedProjectsByCompanyAsync(UnassignedProjectsSpecParams specParams)
        {
            var company = await _unitOfWork.Repository<Company>()
                .FindFirstAsync(c => c.UserId == specParams.CompanyId)
                ?? throw new Exception("Company not found.");

            var spec = new UnassignedProjectsByCompanySpecification(company.Id, specParams);

            var projects = await _unitOfWork.Repository<ProjectEntity>().ListAsync(spec);

            var countSpec = new UnassignedProjectsByCompanySpecification(company.Id, specParams);

            var totalCount = await _unitOfWork.Repository<ProjectEntity>().CountAsync(countSpec);

            var dtos = projects.Select(p => p.ToDto()).ToList();

            return new Pagination<ProjectDto>(
                specParams.PageIndex,
                specParams.PageSize,
                totalCount,
                dtos
            );
        }

        public async Task<List<FileDto>> UploadFilesToProjectAsync(int projectId, List<IFormFile> files, bool isFreelancerUpload)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("No files provided.");

            var project = await _unitOfWork.Repository<ProjectEntity>().FindFirstAsync(p => p.Id == projectId)
                ?? throw new Exception("Project not found or access denied.");

            var uploadedFiles = new List<FileDto>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLower();
                    if (string.IsNullOrEmpty(fileExtension))
                        throw new Exception("Invalid file type.");

                    if (!Enum.TryParse(typeof(FileType), fileExtension, true, out var fileType))
                        throw new Exception($"Unsupported file type: {fileExtension}");

                    var (cloudinaryUrl, publicId) = await UploadToCloudinaryAsync(file);

                    var fileEntity = new Data.Entities.File
                    {
                        FilePath = cloudinaryUrl,
                        ProjectId = project.Id,
                        FileName = file.FileName,
                        FileType = (FileType)fileType,
                        FileSize = file.Length,
                        CreationDate = DateTime.UtcNow,
                        ModificationDate = DateTime.UtcNow,
                        CreatedBy = "project.Company.UserId",
                        ModifiedBy = "project.Company.UserId",
                        PublicId = publicId,
                        IsFreelancerUpload = isFreelancerUpload
                    };

                    await _unitOfWork.Repository<File>().AddAsync(fileEntity);
                    await _unitOfWork.CommitAsync();

                    uploadedFiles.Add(new FileDto
                    {
                        Id = fileEntity.Id,
                        FileName = file.FileName,
                        FilePath = cloudinaryUrl,
                        FileSize = file.Length,
                        FileType = (FileType)fileType,
                        ProjectId = project.Id,
                        IsFreelancerUpload = fileEntity.IsFreelancerUpload
                    });
                }
            }

            await _unitOfWork.Repository<ProjectEntity>().UpdateAsync(project);
            if (!await _unitOfWork.CommitAsync())
                throw new Exception("Failed to upload files to project.");

            return uploadedFiles;
        }

        public async Task DeleteFileAsync(long fileId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("User not found");

            var file = await _unitOfWork.Repository<Data.Entities.File>()
                .FindFirstAsync(f => f.Id == fileId);

            if (file == null)
                throw new Exception("File not found or access denied.");

            if (!string.IsNullOrEmpty(file.PublicId))
                await DeleteFromCloudinaryAsync(file.PublicId);

            await _unitOfWork.Repository<Data.Entities.File>().DeleteAsync(fileId);

            if (!await _unitOfWork.CommitAsync())
                throw new Exception("Failed to delete file.");
        }

        private async Task DeleteFromCloudinaryAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result != "ok")
                throw new Exception("Failed to delete file from Cloudinary.");
        }

        public async Task<RatingDto> CreateRatingAsync(CreateRatingDto dto)
        {
            var spec = new ProjectWithParticipantsSpecification(dto.ProjectId);

            var project = await _unitOfWork.Repository<ProjectEntity>()
                .GetEntityWithSpecification(spec)
                ?? throw new Exception("Project not found.");

            var ratedTo = await _unitOfWork.Repository<ApplicationUser>()
                .FindFirstAsync(u => u.Id == dto.RatedToId)
                ?? throw new Exception("Rated user not found.");

            var ratedBy = await _unitOfWork.Repository<ApplicationUser>()
                .FindFirstAsync(u => u.Id == dto.RatedById)
                ?? throw new Exception("Rater user not found.");

            if (dto.RatingValue < 1 || dto.RatingValue > 5)
                throw new Exception("Rating value must be between 1 and 5.");

            var freelancerUserId = project.Freelancer?.UserId;
            var companyUserId = project.Company?.UserId;

            var isRatedByInProject = dto.RatedById == freelancerUserId || dto.RatedById == companyUserId;
            var isRatedToInProject = dto.RatedToId == freelancerUserId || dto.RatedToId == companyUserId;

            if (!isRatedByInProject || !isRatedToInProject)
                throw new Exception("One or both users are not participants in the specified project.");

            var existingRating = await _unitOfWork.Repository<Rating>()
                .FindFirstAsync(r =>
                    r.ProjectId == dto.ProjectId &&
                    r.RatedById == dto.RatedById &&
                    r.RatedToId == dto.RatedToId);

            if (existingRating != null)
                throw new Exception("You have already rated this user for this project.");

            var rating = new Rating
            {
                RatingValue = dto.RatingValue,
                Review = dto.Review,
                ProjectId = dto.ProjectId,
                RatedById = dto.RatedById,
                RatedToId = dto.RatedToId,
                CreationDate = DateTime.UtcNow,
                CreatedBy = dto.RatedById,
                ModificationDate = DateTime.UtcNow,
                ModifiedBy = dto.RatedById
            };

            await _unitOfWork.Repository<Rating>().AddAsync(rating);
            await _unitOfWork.CommitAsync();

            var ratingDto = rating.ToDto();

            return ratingDto;
        }

        public async Task<APIOperationResponse<TopRatedUsersDto>> GetTopRatedUsersAsync()
        {
            try
            {
                var topFreelancers = await _unitOfWork.Repository<Freelancer>()
                    .GetQueryable()
                    .Include(f => f.User)
                    .Where(f => f.User.RatingsTo.Any())
                    .Select(f => new TopRatedUserDto
                    {
                        UserId = f.UserId,
                        FirstName = f.User.FirstName,
                        LastName = f.User.LastName,
                        ProfileImageUrl = f.User.ProfileImageUrl,
                        UserType = f.User.UserType,
                        AverageRating = f.User.RatingsTo.Average(r => r.RatingValue),
                        TotalRatings = f.User.RatingsTo.Count(),
                        CompletedProjects = f.Projects.Count(p => p.Status == ProjectStatus.Finished)
                    })
                    .OrderByDescending(f => f.AverageRating)
                    .ThenByDescending(f => f.TotalRatings)
                    .Take(5)
                    .ToListAsync();

                var topCompanies = await _unitOfWork.Repository<Company>()
                    .GetQueryable()
                    .Include(c => c.User)
                    .Where(c => c.User.RatingsTo.Any())
                    .Select(c => new TopRatedUserDto
                    {
                        UserId = c.UserId,
                        FirstName = c.User.FirstName,
                        LastName = c.User.LastName,
                        ProfileImageUrl = c.User.ProfileImageUrl,
                        UserType = c.User.UserType,
                        AverageRating = c.User.RatingsTo.Average(r => r.RatingValue),
                        TotalRatings = c.User.RatingsTo.Count(),
                        CompletedProjects = c.Projects.Count(p => p.Status == ProjectStatus.Finished)
                    })
                    .OrderByDescending(c => c.AverageRating)
                    .ThenByDescending(c => c.TotalRatings)
                    .Take(5)
                    .ToListAsync();

                var result = new TopRatedUsersDto
                {
                    TopFreelancers = topFreelancers,
                    TopCompanies = topCompanies
                };

                return APIOperationResponse<TopRatedUsersDto>.Success(result, "Top rated users retrieved successfully.");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<TopRatedUsersDto>.ServerError(
                    "Failed to retrieve top rated users.",
                    new List<string> { ex.Message });
            }
        }
    }
}