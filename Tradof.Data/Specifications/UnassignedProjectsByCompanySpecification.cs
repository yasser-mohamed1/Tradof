using Tradof.Data.Entities;
using Tradof.Data.SpecificationParams;

namespace Tradof.Data.Specifications
{
    public class UnassignedProjectsByCompanySpecification : BaseSpecification<Project>
    {
        public UnassignedProjectsByCompanySpecification(long companyId, UnassignedProjectsSpecParams specParams)
    : base(p =>
        p.CompanyId == companyId &&
        p.FreelancerId == null &&
        (!specParams.SpecializationId.HasValue || p.SpecializationId == specParams.SpecializationId.Value) &&
        (!specParams.LanguageFromId.HasValue || p.LanguageFromId == specParams.LanguageFromId.Value) &&
        (!specParams.LanguageToId.HasValue || p.LanguageToId == specParams.LanguageToId.Value) &&
        (!specParams.DeliveryTimeInDays.HasValue || p.Days <= specParams.DeliveryTimeInDays.Value) &&
        (!specParams.Budget.HasValue || p.MaxPrice <= specParams.Budget.Value))
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

            if (specParams.PageIndex > 0 && specParams.PageSize > 0)
            {
                ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
            }
        }
    }
}