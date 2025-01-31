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
    }
}
