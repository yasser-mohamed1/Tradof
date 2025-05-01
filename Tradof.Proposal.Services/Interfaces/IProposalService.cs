using Tradof.Common.Enums;
using Tradof.Data.SpecificationParams;
using Tradof.EntityFramework.RequestHelpers;
using Tradof.Proposal.Services.DTOs;

namespace Tradof.Proposal.Services.Interfaces
{
    public interface IProposalService
    {
        Task<Pagination<ProposalDto>> GetAllAsync(ProposalSpecParams specParams);
        Task<ProposalDto> GetByIdAsync(long id);
        Task<ProposalDto> CreateAsync(CreateProposalDto dto);
        Task<ProposalDto> UpdateAsync(UpdateProposalDto dto);
        Task<bool> DeleteAsync(long id);
        Task<bool> AcceptProposal(long projectId, long ProposalId);
        Task<bool> DenyProposal(long projectId, long ProposalId);
        Task<bool> CancelProposal(long proposalId);
        Task<List<ProposalGroupResult>> GetProposalsCountAsync(int? year, int? month, ProposalStatus? status);
        Task<Pagination<ProposalDto>> GetFreelancerProposalsAsync(FreelancerProposalsSpecParams specParams);
        Task<bool> AcceptProposalEditAsync(long Id);
        Task<bool> DenyProposalEditAsync(long Id);
        Task<ProposalEditRequestDto> CreateProposalEditAsync(ProposalEditRequestDto dto);
    }
}