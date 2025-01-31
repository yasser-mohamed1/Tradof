namespace Tradof.EntityFramework.RequestHelpers
{
    public class Pagination<T>(int pageIndex, int pageSize, int count, IReadOnlyList<T> items)
    {
        public int PageIndex { get; set; } = pageIndex;
        public int PageSize { get; set; } = pageSize;
        public int Count { get; set; } = count;
        public IReadOnlyList<T> Items { get; set; } = items;
    }
}
