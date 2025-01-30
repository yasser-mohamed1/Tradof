using Tradof.Data.Entities;
using Tradof.Data.SpecificationParams;

namespace Tradof.Data.Specifications
{
    public class ProjectFilterSortPaginationSpecification : BaseSpecification<Project>
    {
        public ProjectFilterSortPaginationSpecification(ProjectSpecParams specParams) : base(project =>
            (string.IsNullOrEmpty(specParams.Search) || project.Name.ToLower().Contains(specParams.Search)) &&
            (specParams._specialization == null || specParams._specialization.Count == 0 || specParams._specialization.Contains((long)project.SpecializationId)) &&
            (specParams.Days == null || specParams.Days.Count == 0 || specParams.Days.Contains(project.Days)) &&
            (!specParams.LanguageFromId.HasValue || project.LanguageFromId == specParams.LanguageFromId) &&
            (!specParams.LanguageToId.HasValue || project.LanguageToId == specParams.LanguageToId) &&
            (!specParams.Budget.HasValue || (specParams.Budget >= project.MinPrice && specParams.Budget <= project.MaxPrice))
        )
        {
            AddInclude(p => p.Files);
            //AddInclude(p => p.Proposals);
            //AddInclude(p => p.Specialization);
            //AddInclude(p => p.LanguageFrom);
            //AddInclude(p => p.LanguageTo);
            //AddInclude(p => p.Company);
            //AddInclude(p => p.Freelancer);
            //AddInclude(p => p.Ratings);

            ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
            switch (specParams.SortBy)
            {
                case "asc":
                    SetOrderBy(p => p.Price);
                    break;
                case "desc":
                    SetOrderByDescending(p => p.Price);
                    break;
                default:
                    SetOrderBy(p => p.Name);
                    break;
            }
        }
    }
}
