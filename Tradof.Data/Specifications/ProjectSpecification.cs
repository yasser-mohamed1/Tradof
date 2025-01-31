using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class ProjectSpecification : BaseSpecification<Project>
    {
        public ProjectSpecification(long id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Files);
            //AddInclude(p => p.Proposals);
            //AddInclude(p => p.Specialization);
            //AddInclude(p => p.LanguageFrom);
            //AddInclude(p => p.LanguageTo);
            //AddInclude(p => p.Company);
            //AddInclude(p => p.Freelancer);
            //AddInclude(p => p.Ratings);
        }
    }
}
