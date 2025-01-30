using Tradof.Common.Enums;

namespace Tradof.Project.Services.DTOs
{
    public class ProjectDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Days { get; set; }
        public long LanguageFromId { get; set; }
        public long LanguageToId { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double? Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public long? SpecializationId { get; set; }
        public int NumberOfOffers { get; set; }
        public List<FileDto>? Files { get; set; }

    }
}
