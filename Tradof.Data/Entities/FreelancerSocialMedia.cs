using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class FreelancerSocialMedia
	{
		public string freelancer_id { get; set; }
		public PlatformType platform_type { get; set; }
		public string Link { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[ForeignKey("freelancer_id")]
		public Freelancer Freelancer { get; set; }

 	}
}
