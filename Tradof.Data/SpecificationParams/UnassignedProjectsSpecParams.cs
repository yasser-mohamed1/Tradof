namespace Tradof.Data.SpecificationParams
{
    public class UnassignedProjectsSpecParams
    {
        public string CompanyId { get; set; } = null!;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 6;

        public int? SpecializationId { get; set; }
        public int? LanguageFromId { get; set; }
        public int? LanguageToId { get; set; }
        public int? DeliveryTimeInDays { get; set; }
        public double? Budget { get; set; }
    }
}