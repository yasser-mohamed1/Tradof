﻿namespace Tradof.Proposal.Services.DTOs
{
    public class CreateProposalEditRequestDto
    {
        public int? NewDuration { get; set; }
        public double? NewPrice { get; set; }
        public long ProposalId { get; set; }
    }
}
