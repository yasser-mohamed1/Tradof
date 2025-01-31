using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Freelancer : AuditEntity<long>
    {
        public string UserId { get; set; }
        public int WorkExperience { get; set; }
        public long CountryId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        public ICollection<Specialization> Specializations = [];
        public ICollection<FreelancerLanguagesPair> FreelancerLanguagesPairs { get; set; } = new List<FreelancerLanguagesPair>();
        public ICollection<FreelancerSocialMedia> FreelancerSocialMedias { get; set; } = new List<FreelancerSocialMedia>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
        public ICollection<WorksOn> WorksOns { get; set; } = new List<WorksOn>();
    }
}