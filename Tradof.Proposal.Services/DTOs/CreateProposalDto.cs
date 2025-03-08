using Microsoft.AspNetCore.Http;

namespace Tradof.Proposal.Services.DTOs
{
    public class CreateProposalDto
    {
        public double OfferPrice { get; set; }
        public long ProjectId { get; set; }
        public string ProposalDescription { get; set; }
        public int Days { get; set; }
        public List<IFormFile> ProposalAttachments { get; set; } = [];
    }
}
