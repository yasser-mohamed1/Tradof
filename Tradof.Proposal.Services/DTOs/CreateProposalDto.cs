namespace Tradof.Proposal.Services.DTOs
{
    public class CreateProposalDto
    {
        public double OfferPrice { get; set; }
        public long ProjectId { get; set; }
        public string ProposalDescription { get; set; }
        public int ProjecDeliveryTime { get; set; }
        public List<string> ProposalAttachments { get; set; } = [];
    }
}
