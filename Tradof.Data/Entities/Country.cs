using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Country : AuditEntity<long>
    {
        public string Name { get; set; }

        public ICollection<Company> Companies { get; set; } = [];
        public ICollection<Freelancer> Freelancers { get; set; } = [];
    }
}
