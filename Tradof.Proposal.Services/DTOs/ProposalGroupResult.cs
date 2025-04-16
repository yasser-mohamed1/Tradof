using Tradof.Common.Enums;

namespace Tradof.Proposal.Services.DTOs
{
    public class ProposalGroupResult
    {
        public int Key { get; set; } // Month or Day
        public required Dictionary<string, int> StatusCounts { get; set; }
    }
}
