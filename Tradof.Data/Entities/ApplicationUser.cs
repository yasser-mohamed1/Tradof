using Microsoft.AspNetCore.Identity;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string first_name { get; set; }
		public string last_name { get; set; }
		public string phone_number { get; set; }
		public string user_image { get; set; }
		public UserType user_type {  get; set; } 
		public string group_name { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		public virtual ICollection<NotificationReceiver> NotificationReceivers { get; set; }
		public virtual ICollection<Feedback> Feedbacks { get; set; }
		public virtual ICollection<TechnicalSupport> TechnicalSupports { get; set; }
		public virtual ICollection<Rating> Ratings { get; set; }
	}
}
