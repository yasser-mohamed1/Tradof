using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    [Table("Freelancers")]
    public class Freelancer : AuditEntity<long>
    {
        public string UserId { get; set; }
        public int WorkExperience { get; set; }
        public long CountryId { get; set; }
        public string? CVFilePath { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        public ICollection<Specialization> Specializations { get; set; } = [];
        public ICollection<FreelancerLanguagesPair> FreelancerLanguagesPairs { get; set; } = [];
        public ICollection<FreelancerSocialMedia> FreelancerSocialMedias { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<Proposal> Proposals { get; set; } = [];
        public ICollection<WorksOn> WorksOns { get; set; } = [];
        public ICollection<PaymentMethod> PaymentMethods { get; set; } = [];
    }
}