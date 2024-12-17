using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class NotificationReceiver
	{		
		public string user_id { get; set; }
		public string notification_id { get; set; }
		public bool message_is_read { get; set; }
		public DateTime received_at { get; set; }

		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[ForeignKey("notification_id")]
		public virtual Notification Notification { get; set; }
		[ForeignKey("user_id")]
		public virtual ApplicationUser User { get; set; }
	}
}
