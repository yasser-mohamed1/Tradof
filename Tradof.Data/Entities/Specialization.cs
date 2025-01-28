using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Specialization : AuditEntity<long>
    {
        public string Name { get; set; }

        public ICollection<Company> Companies { get; set; } = new List<Company>();
        public ICollection<Freelancer> Freelancers { get; set; } = new List<Freelancer>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}