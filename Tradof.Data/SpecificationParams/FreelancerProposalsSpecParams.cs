namespace Tradof.Data.SpecificationParams
{
    public class FreelancerProposalsSpecParams
    {
        private string _freelancerId = string.Empty;
        public string FreelancerId
        {
            get => _freelancerId;
            set => _freelancerId = value;
        }
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }
        private int _pageSize = 6;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }
    }
}
