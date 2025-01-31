using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Notification : AuditEntity<long>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTime ViewedAt { get; set; }

        public ICollection<NotificationReceiver> NotificationReceivers { get; set; } = [];
    }
}