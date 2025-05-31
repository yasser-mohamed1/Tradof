using Tradof.Common.Enums;

namespace Tradof.Project.Services.DTOs
{
    public class TopRatedUserDto
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public UserType UserType { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public int CompletedProjects { get; set; }
    }
}
