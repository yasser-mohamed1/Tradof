using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class Rating : AuditEntity<long>
	{
		public string RatingValue { get; set; }
		public string? Review { get; set; }
		public long ProjectId { get; set; }
		public string RatedById { get; set; }
		public string RatedToId { get; set; }

		[ForeignKey("ProjectId")]
		public Project Project { get; set; }

		[ForeignKey("RatedById")]
		public ApplicationUser RaterBy { get; set; }

		[ForeignKey("RatedToId")]
		public ApplicationUser RaterTo { get; set; }
	}
}