using System.ComponentModel.DataAnnotations;

namespace Tradof.Proposal.Services.DTOs
{
    public class ProposalAttachmentDto
    {
        public long ProposalId { get; set; }
        [Required]
        public string Attachment { get; set; }
    }
}
