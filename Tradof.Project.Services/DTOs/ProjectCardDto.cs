namespace Tradof.Project.Services.DTOs
{
    public class ProjectCardDto
    {
        public string ProjectState { get; set; } = string.Empty;
        public DateTime ProjectStartDate { get; set; }
        public Budget Budget { get; set; } = new();
        public int Duration { get; set; }
        public int AverageOffers { get; set; }
        public int NumberOfOffers { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public int TotalProjects { get; set; }
        public int OpenProjects { get; set; }

    }

    public class Budget
    {
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
    }
}