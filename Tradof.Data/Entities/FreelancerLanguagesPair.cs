using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class FreelancerLanguagesPair : AuditEntity<long>
    {
        public long FreelancerId { get; set; }
        public long LanguageFromId { get; set; }
        public long LanguageToId { get; set; }

        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; }

        [ForeignKey("LanguageFromId")]
        public Language LanguageFrom { get; set; }

        [ForeignKey("LanguageToId")]
        public Language LanguageTo { get; set; }
    }
}