using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Package : AuditEntity<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int DurationInMonths { get; set; }

        public ICollection<CompanySubscription> Subscriptions { get; set; } = [];
    }
}