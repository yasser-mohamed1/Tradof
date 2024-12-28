using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class TechnicalSupport: AuditEntity<long>
	{
		
		public string AgentName {  get; set; }
	

		public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

	}
}
