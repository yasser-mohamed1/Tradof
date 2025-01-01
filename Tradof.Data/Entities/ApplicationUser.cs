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

        public virtual ICollection<NotificationReceiver> NotificationReceivers { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<TechnicalSupport> TechnicalSupports { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}