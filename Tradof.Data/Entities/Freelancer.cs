using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Freelancer : AuditEntity
    {
        public string FreelancerId { get; set; }
        public int WorkExperience { get; set; }
        public Gender Gender { get; set; }
        public long SpecializationId { get; set; }

        [ForeignKey("SpecializationId")]
        public Specialization Specialization { get; set; }

        [ForeignKey("FreelancerId")]
        public ApplicationUser User { get; set; }
        public virtual ICollection<FreelancerLanguagesPair> FreelancerLanguagesPairs { get; set; } = new List<FreelancerLanguagesPair>();
        public virtual ICollection<FreelancerSocialMedia> FreelancerSocialMedias { get; set; } = new List<FreelancerSocialMedia>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}