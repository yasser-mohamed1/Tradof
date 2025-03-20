using Tradof.Common.Enums;

namespace Tradof.Proposal.Services.DTOs
{
    public class ProposalDto
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }

        public long ProjectId { get; set; }
        public long FreelancerId { get; set; }
        public ProposalStatus ProposalStatus { get; set; }
        public string ProposalDescription { get; set; }
        public int Days { get; set; }
        public string? FreelancerName { get; set; }
        public double OfferPrice { get; set; }
        public string? FreelancerImageUrl { get; set; }
        public string? Projecttitle { get; set; }
        public string? CompanyImage { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyFirstName { get; set; }
        public string CompanyLastName { get; set; }
        public DateTime TimePosted { get; set; }
        public ICollection<ProposalAttachmentDto> ProposalAttachments { get; set; } = [];
    }
}
