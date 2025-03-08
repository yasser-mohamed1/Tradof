using Tradof.Data.Entities;
using Tradof.Proposal.Services.DTOs;

namespace Tradof.Proposal.Services.Extensions
{
    public static class ProposalAttachmentExtentions
    {
        public static ProposalAttachmentDto ToDto(this ProposalAttachments attachment)
        {
            return new ProposalAttachmentDto
            {
                AttachmentUrl = attachment.AttachmentUrl,
                ProposalId = attachment.ProposalId
            };
        }
    }
}
