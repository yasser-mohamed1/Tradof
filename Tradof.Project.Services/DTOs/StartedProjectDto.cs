namespace Tradof.Project.Services.DTOs
{
    public class StartedProjectDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Days { get; set; }
        public LanguageDto LanguageFrom { get; set; }
        public LanguageDto LanguageTo { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double? Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectStatusDto Status { get; set; }
        public SpecializationDto? Specialization { get; set; }
        public int NumberOfOffers { get; set; }
        public List<FileDto>? Files { get; set; }
        public string CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FreelancerFirstName { get; set; }
        public string FreelancerLastName { get; set; }
        public string? FreelancerId { get; set; }
        public string JobTitle { get; set; }
        public string ProfileImageUrl { get; set; }
        public string FreelancerProfileImageUrl { get; set; }
        public string FreelancerEmail { get; set; }
        public DateTime CreationDate { get; set; }
    }
}