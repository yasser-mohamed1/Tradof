namespace Tradof.Data.SpecificationParams
{
    public class ProjectSpecParams
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 6;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public List<long>? _specialization = [];

        private string? _search;
        public double? Budget { get; set; }

        public string? Search
        {
            get => _search ?? "";
            set => _search = value?.ToLower();
        }
        public List<int>? Days { get; set; }

        public long? LanguageFromId { get; set; }
        public long? LanguageToId { get; set; }
        public string? SortBy { get; set; }


    }
}
