using Tradof.Data.Entities;
using Tradof.Data.SpecificationParams;

namespace Tradof.Data.Specifications
{
    public class FreelancerProposalsFilterSortPaginationSpecification : BaseSpecification<Proposal>
    {
        public FreelancerProposalsFilterSortPaginationSpecification(FreelancerProposalsSpecParams specParams) : base(proposal =>
        (proposal.Freelancer.UserId == specParams.FreelancerId)
        )
        {
            AddInclude(p => p.ProposalAttachments);
            AddInclude(p => p.Freelancer);
            AddInclude(p => p.Project);
            ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }
    }
}
