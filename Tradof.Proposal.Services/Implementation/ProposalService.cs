using System.ComponentModel.DataAnnotations;
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
        public async Task<ProposalDto> CreateAsync(CreateProposalDto dto)
        {
            var currentUser = await _userHelpers.GetCurrentUserAsync() ?? throw new Exception("current user not found");
            var freelancer = await _unitOfWork.Repository<Freelancer>().FindFirstAsync(f => f.UserId == currentUser.Id);
            var project = await _unitOfWork.Repository<Project>().FindFirstAsync(f => f.Id == dto.ProjectId);

            var proposal = dto.ToEntity();
            proposal.Project = project;
            proposal.Freelancer = freelancer;
            foreach (var attachment in dto.ProposalAttachments)
            {
                proposal.ProposalAttachments.Add(new ProposalAttachments
                {
                    Attachment = attachment,
                    Proposal = proposal,
                    CreatedBy = currentUser.FirstName,
                    CreationDate = DateTime.UtcNow,
                    ModificationDate = DateTime.UtcNow,
                    ModifiedBy = currentUser.FirstName,
                });
            }
            proposal.CreatedBy = currentUser.FirstName;
            proposal.CreationDate = DateTime.UtcNow;
            proposal.ModificationDate = DateTime.UtcNow;
            proposal.ModifiedBy = currentUser.FirstName;
            await _unitOfWork.Repository<Data.Entities.Proposal>().AddAsync(proposal);
            if (await _unitOfWork.CommitAsync())
                return proposal.ToDto();
            else
                throw new Exception("failed to create");
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
            var proposal = await _unitOfWork.Repository<Data.Entities.Proposal>().GetByIdAsync(dto.Id, includes: [p => p.Freelancer, p => p.Project]) ?? throw new NotFoundException("proposal not found");
            await _unitOfWork.Repository<ProposalAttachments>().DeleteWithCrateriaAsync(p => p.ProposalId == proposal.Id);
            proposal.ProposalAttachments.Clear();
            await _unitOfWork.CommitAsync();
            proposal.UpdateFromDto(dto);
            foreach (var attachment in dto.ProposalAttachments)
            {
                proposal.ProposalAttachments.Add(new ProposalAttachments
                {
                    Attachment = attachment,
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
            else
                throw new Exception("failed to update");
        }
    }
}
