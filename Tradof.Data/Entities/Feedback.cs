using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class Feedback
	{
		public string feedback_id { get; set; } = Guid.NewGuid().ToString();
		public double rating { get; set; }
		public string? reson_rating { get; set; }
		public string? idea {  get; set; }	
		public string user_id { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[ForeignKey("user_id")]
		public virtual ApplicationUser User { get; set; }
	}
}
