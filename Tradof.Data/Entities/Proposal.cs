using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Proposal : AuditEntity<long>
    {
        public string ProposalId { get; set; } = Guid.NewGuid().ToString();
        public string ProjectId { get; set; }
        public string FreelancerId { get; set; }
        public ProposalStatus ProposalStatus { get; set; }
        public string ProposalDescription { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; }
    }
}