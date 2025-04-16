using Tradof.Data.Entities;
using Tradof.Data.SpecificationParams;

namespace Tradof.Data.Specifications
{
    public class ProposalFilterSortPaginationSpecification : BaseSpecification<Proposal>
    {
        public ProposalFilterSortPaginationSpecification(ProposalSpecParams specParams) : base(proposal =>
        (specParams.Days == null || proposal.Days <= specParams.Days) &&
        (specParams.OfferPrice == null || proposal.OfferPrice <= specParams.OfferPrice) &&
        (!specParams.ProjectId.HasValue || proposal.ProjectId == specParams.ProjectId)
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
