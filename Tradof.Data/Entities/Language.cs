using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Language : AuditEntity<long>
    {
        public string? LanguageName { get; set; }
        public string? LanguageCode { get; set; }
        public string? CountryName { get; set; }
        public string? CountryCode { get; set; }
        public ICollection<Project> ProjectsLanguageTo { get; set; } = [];
        public ICollection<Project> ProjectsLanguageFrom { get; set; } = [];
        public ICollection<FreelancerLanguagesPair> LanguagePairsTo { get; set; } = [];
        public ICollection<FreelancerLanguagesPair> LanguagePairsFrom { get; set; } = [];
        public ICollection<Company> Companies { get; set; } = [];
    }
}
