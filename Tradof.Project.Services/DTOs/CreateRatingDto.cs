namespace Tradof.Project.Services.DTOs
{
    public class CreateRatingDto
    {
        public double RatingValue { get; set; }
        public string? Review { get; set; }
        public long ProjectId { get; set; }
        public string RatedToId { get; set; }
        public string RatedById { get; set; }
    }
}
