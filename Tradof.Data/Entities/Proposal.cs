using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Proposal
	{
		public string proposal_id { get; set; }
		public double price { get; set; }
		public PropsalStatus propsal_type { get; set; }
		public string notes { get; set; }
		public DateTime estimated_date { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public string freelancer_id { get; set; }
		[ForeignKey("freelancer_id")]
		public Freelancer Freelancer { get; set; }	
		public virtual ICollection<PropsalAttachments> PropsalAttachments { get; set; } = new List<PropsalAttachments>();


	}
}
