using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class CompanyEmployee : AuditEntity<long>
    {
        public string JobTitle { get; set; }
        public CompanyUserType UserType { get; set; }
        public GroupName GroupName { get; set; }
        public string UserId { get; set; }
        public long CountryId { get; set; }
        public long CompanyId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}