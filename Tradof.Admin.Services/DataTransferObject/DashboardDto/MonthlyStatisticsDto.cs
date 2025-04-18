namespace Tradof.Admin.Services.DataTransferObject.DashboardDto
{
    public class MonthlyStatisticsDto
    {
        public string Month { get; set; }
        public int Companies { get; set; }
        public int Freelancer { get; set; }
        public ProjectStat Projects { get; set; }
    }
}