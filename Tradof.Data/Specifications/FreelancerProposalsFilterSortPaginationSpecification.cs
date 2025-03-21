using Tradof.Data.Entities;
using Tradof.Data.SpecificationParams;

namespace Tradof.Data.Specifications
{
    public class FreelancerProposalsFilterSortPaginationSpecification : BaseSpecification<Proposal>
    {
        public FreelancerProposalsFilterSortPaginationSpecification(FreelancerProposalsSpecParams specParams) : base(proposal =>
        (proposal.Freelancer.Id == specParams.FreelancerId) &&
        (specParams.Days == null || proposal.Days <= specParams.Days) &&
        (specParams.OfferPrice == null || proposal.OfferPrice <= specParams.OfferPrice)
        )
        {
            AddInclude(p => p.ProposalAttachments);
            AddInclude(p => p.Freelancer);
            AddInclude(p => p.Freelancer.User);
            AddInclude(p => p.Project);
            AddInclude(p => p.Project.Company);
            AddInclude(p => p.Project.Company.User);
            ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }
    }
}
