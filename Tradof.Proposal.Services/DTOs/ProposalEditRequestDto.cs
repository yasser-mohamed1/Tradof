namespace Tradof.Proposal.Services.DTOs
{
    public class ProposalEditRequestDto
    {
        public long Id { get; set; }
        public int? NewDuration { get; set; }
        public double? NewPrice { get; set; }
    }
}
