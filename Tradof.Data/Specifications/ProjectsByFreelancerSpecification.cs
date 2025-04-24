using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class ProjectsByFreelancerSpecification : BaseSpecification<Project>
    {
        public ProjectsByFreelancerSpecification(long freelancerId, int? pageIndex = null, int? pageSize = null)
            : base(p => p.FreelancerId == freelancerId)
        {
            AddInclude(p => p.Files);
            AddInclude(p => p.Proposals);
            AddInclude(p => p.Specialization);
            AddInclude(p => p.LanguageFrom);
            AddInclude(p => p.LanguageTo);
            AddInclude(p => p.Company);
            AddInclude(p => p.Company.User);
            AddInclude(p => p.Freelancer);
            AddInclude(p => p.Freelancer.User);
            AddInclude(p => p.Ratings);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                ApplyPagination((pageIndex.Value - 1) * pageSize.Value, pageSize.Value);
            }
        }
    }

}
