using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class Rating:AuditEntity<long>
	{
		public string rating { get; set; }
		public string? review { get; set; }
		public string project_id { get; set; }
		public string rated_by_id { get; set; }
		public string rated_to_id { get; set; }

	

		[ForeignKey("project_id")]
		public Project Project { get; set; }

		[ForeignKey("rated_by_id")]
		public ApplicationUser RaterBy { get; set; }

		[ForeignKey("rated_to_id")]
		public ApplicationUser RaterTo { get; set; }
	}
}
