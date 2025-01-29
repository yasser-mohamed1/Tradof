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
        public GroupName GroupName { get; set; }
        public long CountryId { get; set; }
        public string UserId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }


        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<Specialization> Specializations = [];
        public ICollection<CompanySubscription> Subscriptions { get; set; } = new List<CompanySubscription>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Language> PreferredLanguages = [];
    }
}