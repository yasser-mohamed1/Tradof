using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class PackageFeature : AuditEntity<long>
    {
        public string PackageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [ForeignKey("PackageId")]
        public Package Package { get; set; }
    }
}