using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class CompanySocialMedia : AuditEntity<long>
    {
        public PlatformType PlatformType { get; set; }
        public string Link { get; set; }
        public long CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}