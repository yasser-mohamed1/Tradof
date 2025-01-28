using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Feedback : AuditEntity<long>
    {
        public double Rating { get; set; }
        public string? ReasonRating { get; set; }
        public string? Idea { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}