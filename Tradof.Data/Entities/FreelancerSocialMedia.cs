using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class FreelancerSocialMedia: AuditEntity<long>
    {
		public string freelancer_id { get; set; }
		public PlatformType platform_type { get; set; }
		public string Link { get; set; }
	

		[ForeignKey("freelancer_id")]
		public Freelancer Freelancer { get; set; }

 	}
}
