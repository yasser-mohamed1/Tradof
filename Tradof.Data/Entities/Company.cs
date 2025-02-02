using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Company : AuditEntity<long>
    {
        public string? CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public long CountryId { get; set; }
        public string UserId { get; set; }
        public string JobTitle { get; set; }
        public CompanyUserType UserType { get; set; }
        public GroupName GroupName { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<Specialization> Specializations { get; set; } = [];
        public ICollection<CompanySubscription> Subscriptions { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = [];
        public ICollection<Language> PreferredLanguages = [];
        public ICollection<CompanySocialMedia> Medias = [];
        public ICollection<CompanyEmployee> Employees = [];
    }
}