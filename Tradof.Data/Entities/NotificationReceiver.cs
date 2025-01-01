using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class NotificationReceiver : AuditEntity<long>
    {
        public long NotificationId { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; }

        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}