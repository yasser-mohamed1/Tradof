using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class TechnicalSupport : AuditEntity<long>
    {
        public int AgentName { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }

        public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
    }
}