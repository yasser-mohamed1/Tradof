using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class WorksOn : AuditEntity<long>
    {
        [Required]
        public long FreelancerId { get; set; }

        [Required]
        public long ProjectId { get; set; }

        [ForeignKey("FreelancerId")]
        public virtual Freelancer Freelancer { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
