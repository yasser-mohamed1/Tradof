using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Freelancer: AuditEntity
    {
		public string freelancer_id { get; set; } = Guid.NewGuid().ToString();
		public string specialization { get; set; }
		public Gender gender { get; set; }
		public int work_experience { get; set; }
		

		[ForeignKey("freelancer_id")]
		public ApplicationUser User { get; set; }
		public virtual ICollection<FreelancerLanguagesPair> FreelancerLanguagesPairs { get; set; } = new List<FreelancerLanguagesPair>();
		public virtual ICollection<FreelancerSocialMedia> FreelancerSocialMedias { get; set; } = new List<FreelancerSocialMedia>();
		public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
		public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
	}
}
