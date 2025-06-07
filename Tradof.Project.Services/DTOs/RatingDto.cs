namespace Tradof.Project.Services.DTOs
{
    public class RatingDto
    {
        public long Id { get; set; }
        public double RatingValue { get; set; }
        public string? Review { get; set; }
        public long ProjectId { get; set; }
        public string RatedToId { get; set; }
        public string RatedById { get; set; }
        public string RatedByName { get; set; }
        public string RatedToName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
