namespace Tradof.Admin.Services.DataTransferObject.DashboardDto
{
    public class StatisticsDto
    {
        public int[] FreelancersPerMonth { get; set; } = new int[12];
        public int[] ProjectsPerMonth { get; set; } = new int[12];
        public int[] CompaniesPerMonth { get; set; } = new int[12];

        public int[] FreelancersPerDay { get; set; } = new int[31];
        public int[] ProjectsPerDay { get; set; } = new int[31];
        public int[] CompaniesPerDay { get; set; } = new int[31];

        public int[] FreelancersPerWeek { get; set; } = new int[7];
        public int[] ProjectsPerWeek { get; set; } = new int[7];
        public int[] CompaniesPerWeek { get; set; } = new int[7];
    }
}