using Tradof.Common.Enums;
using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class PendingProjectsByCompanySpecification : BaseSpecification<Project>
    {
        public PendingProjectsByCompanySpecification(long companyId)
            : base(p => p.Status == ProjectStatus.Pending && p.CompanyId == companyId)
        {
            AddInclude(p => p.Files);
            AddInclude(p => p.Specialization);
            AddInclude(p => p.LanguageFrom);
            AddInclude(p => p.LanguageTo);
        }
    }
}