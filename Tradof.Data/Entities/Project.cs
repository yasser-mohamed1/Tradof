using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Project
	{
		public string project_id { get; set; } = Guid.NewGuid().ToString();
		public string project_name { get; set; }
		public string description { get; set; }
		public double budget { get; set; }
		public string specialization { get; set; }
		public string language_from { get; set; }
		public string language_to { get; set; }
		public string? internal_node { get; set; }
		public DateTime deadline { get; set; }
		public ProjectStatus project_status { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		public string company_id { get; set; }

		[ForeignKey("company_id")]
		public Company Company { get; set; }
	    public string freelancer_id { get; set; }

		[ForeignKey("freelancer_id")]
		public Freelancer Freelancer { get; set; }

		public virtual ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
		public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
		public virtual ICollection<File> Files { get; set; } = new List<File>();


	}
}
