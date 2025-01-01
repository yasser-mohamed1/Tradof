using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class FreelancerSocialMedia : AuditEntity<long>
    {
        public long FreelancerId { get; set; }
        public PlatformType PlatformType { get; set; }
        public string Link { get; set; }

        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; }
    }
}