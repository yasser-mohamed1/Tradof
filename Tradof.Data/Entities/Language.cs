using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Language : AuditEntity<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public ICollection<Project> ProjectsLanguageTo { get; set; } = new List<Project>();
        public ICollection<Project> ProjectsLanguageFrom { get; set; } = new List<Project>();
        public ICollection<FreelancerLanguagesPair> LanguagePairsTo { get; set; } = new List<FreelancerLanguagesPair>();
        public ICollection<FreelancerLanguagesPair> LanguagePairsFrom { get; set; } = new List<FreelancerLanguagesPair>();
    }
}
