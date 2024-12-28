using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class PropsalAttachments : AuditEntity<long>
    {
		public string proposal_id { get; set; }
		public string Attachment { get; set; }
	

		[ForeignKey("proposal_id")]
		public Proposal Proposal { get; set; }
	}
}
