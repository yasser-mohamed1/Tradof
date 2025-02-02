using Microsoft.AspNetCore.Identity;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public UserType UserType { get; set; }

        public bool IsEmailConfirmed { get; set; }
        public string EmailConfirmationToken { get; set; }


        public ICollection<NotificationReceiver> NotificationReceivers { get; set; } = [];
        public ICollection<Feedback> Feedbacks { get; set; } = [];
        public ICollection<TechnicalSupport> TechnicalSupports { get; set; } = [];
        public ICollection<Rating> RatingsFrom { get; set; } = [];
        public ICollection<Rating> RatingsTo { get; set; } = [];
    }
}