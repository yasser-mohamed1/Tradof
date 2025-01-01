using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Company : AuditEntity<long>
    {
        public string CompanyAddress { get; set; }
        public string JobTitle { get; set; }
        public CompanyUserType UserType { get; set; }
        public long CountryId { get; set; }
        public long SpecializationId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        [ForeignKey("SpecializationId")]
        public Specialization Specialization { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }

        public virtual ICollection<CompanySubscription> Subscriptions { get; set; } = new List<CompanySubscription>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}