using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Specialization : AuditEntity<long>
    {
        public string Name { get; set; }

        public ICollection<Company> Companies { get; set; } = [];
        public ICollection<Freelancer> Freelancers { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = [];
    }
}