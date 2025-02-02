using Tradof.Data.Entities;
using Tradof.Data.SpecificationParams;

namespace Tradof.Data.Specifications
{
    public class ProposalFilterSortPaginationSpecification : BaseSpecification<Proposal>
    {
        public ProposalFilterSortPaginationSpecification(ProposalSpecParams specParams) : base(proposal => specParams.Status == null || specParams.Status == proposal.ProposalStatus)
        {
            AddInclude(p => p.ProposalAttachments);
            AddInclude(p => p.Freelancer);
            AddInclude(p => p.Project);
            ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }
    }
}
