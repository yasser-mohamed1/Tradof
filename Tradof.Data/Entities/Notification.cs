using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Notification
	{
		public string notification_id { get; set; } = Guid.NewGuid().ToString();
		public string message { get; set; }	
		public string? target {  get; set; }
		public NotificationType notification_type { get; set; }
		public DateTime viewed_at { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public virtual ICollection<NotificationReceiver> NotificationReceivers { get; set; }
	}
}
