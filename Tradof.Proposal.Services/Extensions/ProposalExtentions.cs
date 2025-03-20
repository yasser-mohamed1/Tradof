﻿using Tradof.Common.Enums;
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
                Days = proposal.Days,
                Projecttitle = proposal.Project.Name,
                ProposalStatus = proposal.ProposalStatus,
                TimePosted = proposal.TimePosted,
                FreelancerId = proposal.FreelancerId,
                FreelancerImageUrl = proposal.Freelancer.User.ProfileImageUrl,
                FreelancerName = proposal.Freelancer.User.FirstName + " " + proposal.Freelancer.User.LastName,
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
                ProposalStatus = ProposalStatus.Pending,
                Days = proposalDto.Days,
                OfferPrice = proposalDto.OfferPrice,
            };
        }
        public static void UpdateFromDto(this Data.Entities.Proposal proposal, UpdateProposalDto proposalDto)
        {

            proposal.ProjectId = proposalDto.ProjectId;
            proposal.ProposalDescription = proposalDto.ProposalDescription;
            proposal.ProposalStatus = ProposalStatus.Pending;
            proposal.Days = proposalDto.Days;
            proposal.OfferPrice = proposalDto.OfferPrice;

        }
    }
}
