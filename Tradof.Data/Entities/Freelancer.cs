using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Freelancer : AuditEntity<long>
    {
        public string UserId { get; set; }
        public int WorkExperience { get; set; }
        public long CountryId { get; set; }
        public long SpecializationId { get; set; }

        [ForeignKey("SpecializationId")]
        public Specialization Specialization { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        public ICollection<FreelancerLanguagesPair> FreelancerLanguagesPairs { get; set; } = [];
        public ICollection<FreelancerSocialMedia> FreelancerSocialMedias { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public ICollection<WorksOn> WorksOns { get; set; } = new List<WorksOn>();
    }
}