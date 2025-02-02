using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Project : AuditEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public int Days { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double? Price { get; set; }

        public long? FreelancerId { get; set; }
        public long CompanyId { get; set; }
        public long? SpecializationId { get; set; }
        public long LanguageFromId { get; set; }
        public long LanguageToId { get; set; }

        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; }

        [ForeignKey("SpecializationId")]
        public Specialization Specialization { get; set; }

        [ForeignKey("LanguageFromId")]
        public Language LanguageFrom { get; set; }

        [ForeignKey("LanguageToId")]
        public Language LanguageTo { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public ICollection<File> Files { get; set; } = [];
        public ICollection<Rating> Ratings { get; set; } = [];
        public ICollection<Proposal> Proposals { get; set; } = [];
    }
}