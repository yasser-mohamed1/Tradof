using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
using Tradof.Proposal.Services.DTOs;
using Tradof.Proposal.Services.Extensions;
using Tradof.Proposal.Services.Interfaces;

namespace Tradof.Proposal.Services.Implementation
{
    public class ProposalService(IUnitOfWork _unitOfWork, IUserHelpers _userHelpers) : IProposalService
    {
        Cloudinary _cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));

        public async Task<bool> AcceptProposal(long projectId, long proposalId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var project = await _unitOfWork.Repository<Project>().FindFirstAsync(f => f.Id == projectId) ?? throw new Exception("project not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id) ?? throw new NotFoundException("company not found");

            if (company.Id != project.CompanyId)
                throw new Exception("not authorized to accept this");

            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(proposalId) ?? throw new NotFoundException("proposal not found");
            proposal.ProposalStatus = ProposalStatus.Accepted;
            project.FreelancerId = proposal.FreelancerId;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> DenyProposal(long projectId, long proposalId)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var project = await _unitOfWork.Repository<Project>().FindFirstAsync(f => f.Id == projectId) ?? throw new Exception("project not found");
            var company = await _unitOfWork.Repository<Company>().FindFirstAsync(f => f.UserId == currentUser.Id) ?? throw new NotFoundException("company not found");

            if (company.Id != project.CompanyId)
                throw new Exception("not authorized to deny this");

            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(proposalId) ?? throw new NotFoundException("proposal not found");
            proposal.ProposalStatus = ProposalStatus.Declined;
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
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id);
            var project = await _unitOfWork.Repository<Project>().FindFirstAsync(f => f.Id == dto.ProjectId);

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
                return proposal.ToDto();

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
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(dto.Id, includes: [p => p.Freelancer, p => p.Project]) ?? throw new NotFoundException("Proposal not found");

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

        public async Task<int> GetProposalsCountByMonth(int? year, int? month, ProposalStatus? status)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("Current user not found");

            var query = _unitOfWork.Repository<Data.Entities.Proposal>()
                .GetQueryable()
                .Where(p => p.Freelancer.UserId == currentUser.Id && p.ProposalStatus == status);

            if (year.HasValue)
                query = query.Where(p => p.TimePosted.Year == year.Value);

            if (month.HasValue)
                query = query.Where(p => p.TimePosted.Month == month.Value);

            return await query.CountAsync();
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
    }
}