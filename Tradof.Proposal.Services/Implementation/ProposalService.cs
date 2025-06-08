using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
using Tradof.Project.Services.Interfaces;
using Tradof.Proposal.Services.DTOs;
using Tradof.Proposal.Services.Extensions;
using Tradof.Proposal.Services.Interfaces;

namespace Tradof.Proposal.Services.Implementation
{
    public class ProposalService(IUnitOfWork _unitOfWork, IUserHelpers _userHelpers, IEmailService _emailService, IBackgroundJobClient _backgroundJob, INotificationService _notificationService) : IProposalService
    {
        Cloudinary _cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));

        public async Task<bool> AcceptProposal(long projectId, long proposalId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var project = await _unitOfWork.Repository<Data.Entities.Project>().FindFirstAsync(f => f.Id == projectId) ?? throw new Exception("project not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id) ?? throw new NotFoundException("company not found");

            if (company.Id != project.CompanyId)
                throw new Exception("not authorized to accept this");

            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(proposalId, includes: [p => p.Freelancer.User]) ?? throw new NotFoundException("proposal not found");
            proposal.ProposalStatus = ProposalStatus.Accepted;
            project.FreelancerId = proposal.FreelancerId;
            project.StartDate = DateTime.UtcNow;
            project.Status = ProjectStatus.Active;
            project.Price = proposal.OfferPrice;
            project.Days = proposal.Days;
            project.EndDate = DateTime.UtcNow.AddDays(proposal.Days);
            project.AcceptedProposalId = proposalId;

            // Send notification to freelancer
            var notification = new NotificationDto
            {
                type = "Offer",
                senderId = currentUser.Id,
                receiverId = proposal.Freelancer.User.Id,
                message = $"Your proposal for project '{project.Name}' has been accepted.",
                description = $"Your offer for project '{project.Name}' was accepted. You can now start working on it.",
                timestamp = DateTime.UtcNow
            };
            await _notificationService.SendNotificationAsync(notification);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> DenyProposal(long projectId, long proposalId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var project = await _unitOfWork.Repository<Data.Entities.Project>().FindFirstAsync(f => f.Id == projectId) ?? throw new Exception("project not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id) ?? throw new NotFoundException("company not found");

            if (company.Id != project.CompanyId)
                throw new Exception("not authorized to deny this");

            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(proposalId, includes: [p => p.Freelancer.User]) ?? throw new NotFoundException("proposal not found");
            proposal.ProposalStatus = ProposalStatus.Declined;

            // Send notification to freelancer
            var notification = new NotificationDto
            {
                type = "Offer",
                senderId = currentUser.Id,
                receiverId = proposal.Freelancer.User.Id,
                message = $"Your proposal for project '{project.Name}' has been rejected.",
                description = $"Your offer for project '{project.Name}' was not accepted.",
                timestamp = DateTime.UtcNow
            };
            await _notificationService.SendNotificationAsync(notification);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> CancelProposal(long proposalId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(proposalId) ?? throw new NotFoundException("proposal not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id) ?? throw new NotFoundException("freelancer not found");

            if (freelancer.Id != proposal.FreelancerId)
                throw new Exception("not authorized to accept this");

            proposal.ProposalStatus = ProposalStatus.Canceled;
            return await _unitOfWork.CommitAsync();
        }

        public async Task<ProposalDto> CreateAsync(CreateProposalDto dto)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("Current user not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id, includes: [p => p.User]);
            var project = await _unitOfWork.Repository<Data.Entities.Project>().FindFirstAsync(f => f.Id == dto.ProjectId, includes: [p => p.Company.User]);

            if (project.Status != ProjectStatus.Pending)
                throw new Exception("Cannot send proposal for this project");

            var existingProposal = await _unitOfWork.Repository<Data.Entities.Proposal>().FindFirstAsync(p => p.FreelancerId == freelancer.Id && p.ProjectId == project.Id);
            if (existingProposal != null) throw new Exception("already sent a proposal for this project");

            var proposal = dto.ToEntity();
            proposal.Project = project;
            proposal.Freelancer = freelancer;

            foreach (var file in dto.ProposalAttachments)
            {
                var uploadedUrl = await UploadToCloudinaryAsync(file);

                proposal.ProposalAttachments.Add(new ProposalAttachments
                {
                    AttachmentUrl = uploadedUrl,
                    Proposal = proposal,
                    CreatedBy = currentUser.FirstName,
                    CreationDate = DateTime.UtcNow,
                    ModificationDate = DateTime.UtcNow,
                    ModifiedBy = currentUser.FirstName
                });
            }

            proposal.CreatedBy = currentUser.FirstName;
            proposal.CreationDate = DateTime.UtcNow;
            proposal.ModificationDate = DateTime.UtcNow;
            proposal.ModifiedBy = currentUser.FirstName;

            await _unitOfWork.Repository<Data.Entities.Proposal>().AddAsync(proposal);

            if (await _unitOfWork.CommitAsync())
            {
                var notification = new NotificationDto
                {
                    type = "Offer",
                    senderId = currentUser.Id,
                    receiverId = project.Company.User.Id,
                    message = $"{freelancer.User.FirstName} {freelancer.User.LastName} sent a proposal for project '{project.Name}'.",
                    description = $"please response for  {freelancer.User.FirstName} {freelancer.User.LastName} on '{project.Name}'.",
                    timestamp = DateTime.UtcNow
                };
                await _notificationService.SendNotificationAsync(notification);
                var specification = new ProposalSpecification(proposal.Id);
                var proposalItem = await _unitOfWork.Repository<Data.Entities.Proposal>().GetEntityWithSpecification(specification);
                return proposalItem.ToDto();
            }

            throw new Exception("Failed to create proposal");
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(id) ?? throw new NotFoundException("proposal not found");

            await _unitOfWork.Repository<Data.Entities.Proposal>().DeleteAsync(proposal.Id);
            await _unitOfWork.Repository<ProposalAttachments>().DeleteWithCrateriaAsync(p => p.ProposalId == proposal.Id);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<Pagination<ProposalDto>> GetAllAsync(ProposalSpecParams specParams)
        {
            var specification = new ProposalFilterSortPaginationSpecification(specParams);
            var items = await _unitOfWork.Repository<Data.Entities.Proposal>().ListAsync(specification);
            var count = await _unitOfWork.Repository<Data.Entities.Proposal>().CountAsync(specification);
            var dtos = items.Select(p => p.ToDto()).ToList();
            var pagination = new Pagination<ProposalDto>(specParams.PageIndex, specParams.PageSize, count, dtos);
            return pagination;
        }

        public async Task<ProposalDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid project ID.");
            var spec = new ProposalSpecification(id);
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetEntityWithSpecification(spec);
            return proposal == null ? throw new NotFoundException("proposal not found") : proposal.ToDto();
        }

        public async Task<ProposalDto> UpdateAsync(UpdateProposalDto dto)
        {
            var proposal = await _unitOfWork.Repository<Tradof.Data.Entities.Proposal>()
                .GetByIdAsync(dto.Id, includes: [p => p.Freelancer, p => p.Freelancer.User, p => p.Project,
                    p => p.Project.Company, p => p.Project.Company.User])
                ?? throw new NotFoundException("Proposal not found");

            await _unitOfWork.Repository<ProposalAttachments>().DeleteWithCrateriaAsync(p => p.ProposalId == proposal.Id);
            proposal.ProposalAttachments.Clear();
            await _unitOfWork.CommitAsync();

            proposal.UpdateFromDto(dto);

            foreach (var file in dto.ProposalAttachments)
            {
                var uploadedUrl = await UploadToCloudinaryAsync(file);

                proposal.ProposalAttachments.Add(new ProposalAttachments
                {
                    AttachmentUrl = uploadedUrl,
                    Proposal = proposal,
                    CreatedBy = proposal.FreelancerId.ToString(),
                    CreationDate = DateTime.UtcNow,
                    ModificationDate = DateTime.UtcNow,
                    ModifiedBy = proposal.FreelancerId.ToString()
                });
            }

            await _unitOfWork.Repository<Data.Entities.Proposal>().UpdateAsync(proposal);

            if (await _unitOfWork.CommitAsync())
                return proposal.ToDto();

            throw new Exception("Failed to update proposal");
        }

        public async Task<List<ProposalGroupResult>> GetProposalsCountAsync(int? year, int? month, ProposalStatus? status)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("Current user not found");

            var query = _unitOfWork.Repository<Data.Entities.Proposal>()
                .GetQueryable()
                .Where(p => p.Freelancer.UserId == currentUser.Id);

            if (status.HasValue)
                query = query.Where(p => p.ProposalStatus == status.Value);

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
                query = query.Where(p => p.TimePosted >= fromDate && p.TimePosted <= toDate);
            }
            if (year.HasValue)
            {
                query = query.Where(p => p.TimePosted.Year == year.Value);
            }
            if (month.HasValue)
            {
                query = query.Where(p => p.TimePosted.Month == month.Value);
            }

            // Group by day or month depending on the request
            if (month.HasValue)
            {
                var groupedData = await query
                    .ToListAsync();

                return groupedData
                    .GroupBy(p => p.TimePosted.Day)
                    .OrderBy(g => g.Key)
                    .Select(g => new ProposalGroupResult
                    {
                        Key = g.Key,
                        StatusCounts = g
                            .GroupBy(p => p.ProposalStatus.ToString())
                            .ToDictionary(s => s.Key, s => s.Count())
                    })
                    .ToList();
            }
            else
            {
                var groupedData = await query
                    .ToListAsync();

                return groupedData
                    .GroupBy(p => p.TimePosted.Month)
                    .OrderBy(g => g.Key)
                    .Select(g => new ProposalGroupResult
                    {
                        Key = g.Key,
                        StatusCounts = g
                            .GroupBy(p => p.ProposalStatus.ToString())
                            .ToDictionary(s => s.Key, s => s.Count())
                    })
                    .ToList();
            }
        }

        public async Task<Pagination<ProposalDto>> GetFreelancerProposalsAsync(FreelancerProposalsSpecParams specParams)
        {
            var specification = new FreelancerProposalsFilterSortPaginationSpecification(specParams);
            var items = await _unitOfWork.Repository<Data.Entities.Proposal>().ListAsync(specification);
            var count = await _unitOfWork.Repository<Data.Entities.Proposal>().CountAsync(specification);
            var dtos = items.Select(p => p.ToDto()).ToList();
            var pagination = new Pagination<ProposalDto>(specParams.PageIndex, specParams.PageSize, count, dtos);
            return pagination;
        }

        public async Task<ProposalEditRequestDto> CreateProposalEditAsync(CreateProposalEditRequestDto dto)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("Current user not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id, includes: [p => p.User]);
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().FindFirstAsync(p => p.Id == dto.ProposalId);
            var project = await _unitOfWork.Repository<Data.Entities.Project>().FindFirstAsync(f => f.Id == proposal.ProjectId, includes: [p => p.Company.User]);
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.Id == project.CompanyId) ?? throw new Exception("company user not found");
            var companyUser = await _unitOfWork.Repository<ApplicationUser>().FindFirstAsync(f => f.Id == company.UserId) ?? throw new Exception("company user not found");

            if (project.FreelancerId != freelancer.Id)
                throw new Exception("not authorized to send requist");

            if (project.Status != ProjectStatus.Active)
                throw new Exception("project cant recieve the edit requist");

            var proposalEdit = new ProposalEditRequest
            {
                FreelancerId = freelancer.Id,
                ProjectId = project.Id,
                CreatedBy = "system",
                ModifiedBy = "system",
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow
            };
            if (dto.NewDuration != null)
                proposalEdit.NewDuration = (int)dto.NewDuration;
            if (dto.NewPrice != null)
                proposalEdit.NewPrice = (double)dto.NewPrice;

            await _unitOfWork.Repository<ProposalEditRequest>().AddAsync(proposalEdit);

            if (await _unitOfWork.CommitAsync())
            {
                var notification = new NotificationDto
                {
                    type = "Offer",
                    senderId = currentUser.Id,
                    receiverId = project.Company.User.Id,
                    message = $"{freelancer.User.FirstName} {freelancer.User.LastName} asks for editing the offer for project '{project.Name}'.",
                    description = $"please response for {freelancer.User.FirstName} {freelancer.User.LastName} edit request for  '{project.Name}'.",
                    timestamp = DateTime.UtcNow
                };
                await _notificationService.SendNotificationAsync(notification);
                string freelancerName = $"{currentUser.FirstName} {currentUser.LastName}";
                _backgroundJob.Enqueue(() => SendProposalEditEmailAsync(companyUser.Email!, freelancerName, project.Name));
                return new ProposalEditRequestDto
                {
                    NewDuration = dto.NewDuration,
                    NewPrice = dto.NewPrice
                };
            }

            throw new Exception("Failed to send the requist");
        }

        public async Task<bool> AcceptProposalEditAsync(long Id)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("Current user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id, includes: [p => p.User]) ?? throw new Exception("company user not found");
            var proposalEdit = await _unitOfWork.Repository<ProposalEditRequest>().FindFirstAsync(p => p.Id == Id) ?? throw new Exception("proposalEdit requist not found");
            var project = await _unitOfWork.Repository<Data.Entities.Project>().FindFirstAsync(f => f.Id == proposalEdit.ProjectId) ?? throw new Exception("project not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.Id == proposalEdit.FreelancerId, includes: [f => f.User]) ?? throw new Exception("freelancer not found");
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().FindFirstAsync(p => p.Id == project.AcceptedProposalId) ?? throw new Exception("proposal not found");

            if (project.FreelancerId != freelancer.Id && project.CompanyId != company.Id)
                throw new Exception("not authorized to accept this");

            if (project.FreelancerId != freelancer.Id)
                throw new Exception("failed : freelancer id not equal project freelancer id");


            if (proposalEdit.NewDuration != null)
            {
                project.Days = proposalEdit.NewDuration;
                proposal.Days = proposalEdit.NewDuration;
            }
            if (proposalEdit.NewPrice != null)
            {
                project.Price = proposalEdit.NewPrice;
                proposal.OfferPrice = proposalEdit.NewPrice;
            }
            proposalEdit.Status = ProposalEditRequestStatus.Approved;
            var notification = new NotificationDto
            {
                type = "Offer",
                senderId = currentUser.Id,
                receiverId = proposal.Freelancer.User.Id,
                message = $"Your offer edit request for project '{project.Name}' has been accepted.",
                description = $"{company.User.FirstName} {company.User.LastName} accepted your offer for project '{project.Name}'. You can now start working on it.",
                timestamp = DateTime.UtcNow
            };
            await _notificationService.SendNotificationAsync(notification);

            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> DenyProposalEditAsync(long Id)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("Current user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id) ?? throw new Exception("Current user not found");
            var proposalEdit = await _unitOfWork.Repository<ProposalEditRequest>().FindFirstAsync(p => p.Id == Id) ?? throw new Exception("Company not found");
            var project = await _unitOfWork.Repository<Data.Entities.Project>().FindFirstAsync(f => f.Id == proposalEdit.ProjectId) ?? throw new Exception("project not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.Id == proposalEdit.FreelancerId) ?? throw new Exception("freelancer not found");
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().FindFirstAsync(p => p.Id == project.AcceptedProposalId) ?? throw new Exception("proposal not found");

            if (project.FreelancerId != freelancer.Id && project.CompanyId != company.Id)
                throw new Exception("not authorized to deny this");

            if (project.FreelancerId != freelancer.Id)
                throw new Exception("failed : freelancer id not equal project freelancer id");

            proposalEdit.Status = ProposalEditRequestStatus.Rejected;

            var notification = new NotificationDto
            {
                type = "Offer",
                senderId = currentUser.Id,
                receiverId = proposal.Freelancer.User.Id,
                message = $"Your offer edit request for project '{project.Name}' has been denied.",
                description = $"{company.User.FirstName} {company.User.LastName} denied your offer for project '{project.Name}'.",
                timestamp = DateTime.UtcNow
            };
            await _notificationService.SendNotificationAsync(notification);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<ProposalEditRequestDto> GetProposalEditRequestAsync(long Id)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("Current user not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id) ?? throw new Exception("company user not found");
            var project = await _unitOfWork.Repository<Data.Entities.Project>().FindFirstAsync(f => f.Id == Id) ?? throw new Exception("project not found");

            if (project.CompanyId != company.Id)
                throw new Exception("not autorized to get this");

            var proposalEdit = await _unitOfWork.Repository<ProposalEditRequest>().FindFirstAsync(f => f.ProjectId == project.Id) ?? throw new Exception("proposal Edit request not found");

            return new ProposalEditRequestDto
            {
                Id = proposalEdit.Id,
                NewDuration = proposalEdit.NewDuration,
                NewPrice = proposalEdit.NewPrice,
                Status = proposalEdit.Status
            };
        }

        private async Task<string> UploadToCloudinaryAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "proposals_attachments",
                    PublicId = $"projects/{Guid.NewGuid()}_{file.FileName}",
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.AbsoluteUri;
            }
        }

        public async Task SendProposalEditEmailAsync(string toEmail, string freelancerName, string projectName)
        {
            string emailBody = $"{freelancerName} requested to edit the proposal on {projectName} project";
            await _emailService.SendEmailAsync(toEmail, "Proposal's Edit Request", emailBody);
        }

    }
}