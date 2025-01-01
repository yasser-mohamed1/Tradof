using Microsoft.AspNetCore.Identity;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string GroupName { get; set; }
        public UserType UserType { get; set; }

        public virtual ICollection<NotificationReceiver> NotificationReceivers { get; set; } = new List<NotificationReceiver>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual ICollection<TechnicalSupport> TechnicalSupports { get; set; } = new List<TechnicalSupport>();
        public virtual ICollection<Rating> RatingsFrom { get; set; } = new List<Rating>();
        public virtual ICollection<Rating> RatingsTo { get; set; } = new List<Rating>();
    }
}