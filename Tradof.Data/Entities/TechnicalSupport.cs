using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class TechnicalSupport
	{
		public string support_id { get; set; } = Guid.NewGuid().ToString();
		public string AgentName {  get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

	}
}
