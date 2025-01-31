using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Proposal : AuditEntity<long>
    {
        public long ProjectId { get; set; }
        public long FreelancerId { get; set; }
        public ProposalStatus ProposalStatus { get; set; }
        public string ProposalDescription { get; set; }
        public int ProjecDeliveryTime { get; set; }
        public double OfferPrice { get; set; }
        public DateTime TimePosted { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; }
        public ICollection<ProposalAttachments> ProposalAttachments { get; set; } = [];
    }
}