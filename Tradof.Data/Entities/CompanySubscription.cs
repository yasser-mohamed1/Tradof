using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class CompanySubscription : AuditEntity<long>
    {
        public long CompanyId { get; set; }
        public long PackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Coupon { get; set; }
        public double NetPrice { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        [ForeignKey("PackageId")]
        public Package Package { get; set; }
    }
}