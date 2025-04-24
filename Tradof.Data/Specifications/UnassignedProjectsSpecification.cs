using Tradof.Data.Entities;
using Tradof.Data.SpecificationParams;

namespace Tradof.Data.Specifications
{
    public class UnassignedProjectsSpecification : BaseSpecification<Project>
    {
        public UnassignedProjectsSpecification(UnassignedProjectsSpecParams specParams)
    : base(p =>
        p.FreelancerId == null &&
        (specParams.SpecializationId == null || p.SpecializationId == specParams.SpecializationId) &&
        (specParams.LanguageFromId == null || p.LanguageFromId == specParams.LanguageFromId) &&
        (specParams.LanguageToId == null || p.LanguageToId == specParams.LanguageToId) &&
        (specParams.DeliveryTimeInDays == null || p.Days <= specParams.DeliveryTimeInDays) &&
        (specParams.Budget == null || p.MaxPrice <= specParams.Budget) &&
        (specParams.CompanyId == null || p.Company.UserId == specParams.CompanyId)
    )
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

            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
        }
    }
}
