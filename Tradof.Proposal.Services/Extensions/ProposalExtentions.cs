using Tradof.Common.Enums;
using Tradof.Proposal.Services.DTOs;

namespace Tradof.Proposal.Services.Extensions
{
    public static class ProposalExtentions
    {
        public static ProposalDto ToDto(this Data.Entities.Proposal proposal)
        {
            return new ProposalDto
            {
                Id = proposal.Id,
                OfferPrice = proposal.OfferPrice,
                ProjecDeliveryTime = proposal.ProjecDeliveryTime,
                ProjecPrice = (double)proposal.Project.Price,
                Projecttitle = proposal.Project.Name,
                ProposalStatus = proposal.ProposalStatus,
                TimePosted = proposal.TimePosted,
                FreelancerId = proposal.FreelancerId,
                ProposalAttachments = proposal.ProposalAttachments.Select(p => p.ToDto()).ToList(),
                ProposalDescription = proposal.ProposalDescription,
                ProjectId = proposal.Project.Id,
            };
        }

        public static Data.Entities.Proposal ToEntity(this CreateProposalDto proposalDto)
        {
            return new Data.Entities.Proposal
            {
                ProjectId = proposalDto.ProjectId,
                ProposalDescription = proposalDto.ProposalDescription,
                ProposalStatus = ProposalStatus.Pinding,
                ProjecDeliveryTime = proposalDto.ProjecDeliveryTime,
                OfferPrice = proposalDto.OfferPrice,
            };
        }
        public static Data.Entities.Proposal ToEntity(this UpdateProposalDto proposalDto)
        {
            return new Data.Entities.Proposal
            {
                ProjectId = proposalDto.ProjectId,
                ProposalDescription = proposalDto.ProposalDescription,
                ProposalStatus = ProposalStatus.Pinding,
                ProjecDeliveryTime = proposalDto.ProjecDeliveryTime,
                OfferPrice = proposalDto.OfferPrice,
            };
        }
    }
}
