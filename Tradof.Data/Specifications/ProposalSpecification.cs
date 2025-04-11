using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class ProposalSpecification : BaseSpecification<Proposal>
    {
        public ProposalSpecification(long id) : base(p => p.Id == id)
        {
            AddInclude(p => p.ProposalAttachments);
            AddInclude(p => p.Freelancer);
            AddInclude(p => p.Freelancer.User);
            AddInclude(p => p.Project);
            AddInclude(p => p.Project.Company);
            AddInclude(p => p.Project.Company.User);
        }
    }
}
