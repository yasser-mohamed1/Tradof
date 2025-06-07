using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class ProposalEditRequest : AuditEntity<long>
    {
        public int NewDuration { get; set; }
        public double NewPrice { get; set; }

        public long? FreelancerId { get; set; }
        public long? ProjectId { get; set; }

        public ProposalEditRequestStatus Status { get; set; } = ProposalEditRequestStatus.Pending;

        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

    }
}
