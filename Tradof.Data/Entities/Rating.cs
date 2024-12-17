using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class Rating
	{
		public string rating_id { get; set; }
		public string rating { get; set; }
		public string? review { get; set; }
		public string project_id { get; set; }
		public string rated_by_id { get; set; }
		public string rated_to_id { get; set; }

		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[ForeignKey("project_id")]
		public Project Project { get; set; }

		[ForeignKey("rated_by_id")]
		public ApplicationUser RaterBy { get; set; }

		[ForeignKey("rated_to_id")]
		public ApplicationUser RaterTo { get; set; }
	}
}
