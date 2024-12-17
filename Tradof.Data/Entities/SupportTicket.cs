

using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class SupportTicket
	{
		public string ticket_id { get; set; } = Guid.NewGuid().ToString();
		public string issue_details { get; set; }
		public SupportTicketType ticket_type { get; set; }
		public string user_id { get; set; }
		public string supporter_id {  get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[ForeignKey("user_id")]
		public virtual ApplicationUser User {  get; set; }

		[ForeignKey("supporter_id")]
		public virtual TechnicalSupport TechnicalSupport { get; set; }

	}
}
