using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class ProposalAttachments : AuditEntity<long>
    {
		public long ProposalId { get; set; }
		public string Attachment { get; set; }

		[ForeignKey("ProposalId")]
		public Proposal Proposal { get; set; }
	}
}