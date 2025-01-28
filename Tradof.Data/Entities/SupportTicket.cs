using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class SupportTicket : AuditEntity<long>
    {
        public string IssueDetails { get; set; }
        public string UserId { get; set; }
        public long SupporterId { get; set; }
        public SupportTicketType TicketType { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("SupporterId")]
        public TechnicalSupport TechnicalSupport { get; set; }
    }
}
