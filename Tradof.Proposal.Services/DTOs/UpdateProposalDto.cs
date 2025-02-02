namespace Tradof.Proposal.Services.DTOs
{
    public class UpdateProposalDto
    {
        public long Id { get; set; }
        public double OfferPrice { get; set; }
        public long ProjectId { get; set; }
        public string ProposalDescription { get; set; }
        public int ProjecDeliveryTime { get; set; }
        public List<string> ProposalAttachments { get; set; } = [];
    }
}
