using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Notification: AuditEntity<long>
    {
		
		public string message { get; set; }	
		public string? target {  get; set; }
		public NotificationType notification_type { get; set; }
		public DateTime viewed_at { get; set; }

		public virtual ICollection<NotificationReceiver> NotificationReceivers { get; set; }
	}
}
