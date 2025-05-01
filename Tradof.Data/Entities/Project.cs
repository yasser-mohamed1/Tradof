using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Project : AuditEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public ProjectStatus Status { get; set; }
        public int Days { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double? Price { get; set; }
        public long? FreelancerId { get; set; }
        public long CompanyId { get; set; }
        public long? SpecializationId { get; set; }
        public long? AcceptedProposalId { get; set; }
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
        [ForeignKey("AcceptedProposalId")]
        public Proposal AcceptedProposal { get; set; }

        public ICollection<File> Files { get; set; } = [];
        [NotMapped]
        public IEnumerable<File> FreelancerUploads => Files?.Where(f => f.IsFreelancerUpload == true);
        public ICollection<Rating> Ratings { get; set; } = [];
        public ICollection<Proposal> Proposals { get; set; } = [];
    }
}