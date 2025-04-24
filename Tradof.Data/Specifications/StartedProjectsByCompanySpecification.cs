using Tradof.Common.Enums;
using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class StartedProjectsByCompanySpecification : BaseSpecification<Project>
    {
        public StartedProjectsByCompanySpecification(long companyId, int? pageIndex = null, int? pageSize = null)
            : base(p => p.CompanyId == companyId && p.FreelancerId != null)
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
            AddInclude(p => p.FreelancerUploads);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                ApplyPagination((pageIndex.Value - 1) * pageSize.Value, pageSize.Value);
            }
        }
    }
}
