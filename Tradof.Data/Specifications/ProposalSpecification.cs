using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class ProposalSpecification : BaseSpecification<Proposal>
    {
        public ProposalSpecification(long id) : base(p => p.Id == id)
        {
            AddInclude(p => p.ProposalAttachments);
            AddInclude(p => p.Freelancer);
            AddInclude(p => p.Project);
        }
    }
}
