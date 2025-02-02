using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class PaymentMethod : AuditEntity<long>
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 5)]
        public string ExpiryDate { get; set; } // Format: MM/YY

        [Required]
        [Range(100, 9999)]
        public int CVV { get; set; }

        public long FreelancerId { get; set; }

        [ForeignKey("FreelancerId")]
        public Freelancer Freelancer { get; set; }
    }
}
