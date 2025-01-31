using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class TechnicalSupport : AuditEntity<long>
    {
        public int AgentName { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public ICollection<SupportTicket> SupportTickets { get; set; } = [];
    }
}