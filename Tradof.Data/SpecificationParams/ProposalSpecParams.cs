using Tradof.Common.Enums;

namespace Tradof.Data.SpecificationParams
{
    public class ProposalSpecParams
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 6;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public ProposalStatus? Status { get; set; }
        public string? SortBy { get; set; }
    }
}
