using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class ProjectWithParticipantsSpecification : BaseSpecification<Project>
    {
        public ProjectWithParticipantsSpecification(long projectId)
            : base(p => p.Id == projectId)
        {
            AddInclude(p => p.Company);
            AddInclude(p => p.Freelancer);
        }
    }
}
