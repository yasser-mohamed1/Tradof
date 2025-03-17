using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class PaymentMethod : AuditEntity<long>
	{
		[Required]
		[CreditCard]
		public string CardNumber { get; set; } // Encrypt this field for security

		[Required]
		[StringLength(5, MinimumLength = 5)]
		public string ExpiryDate { get; set; } // Encrypt this field for security

		[Required]
		[Range(100, 9999)]
		public int CVV { get; set; } // Encrypt this field for security

		public long FreelancerId { get; set; } // Foreign key to Freelancer

		[ForeignKey("FreelancerId")]
		public Freelancer Freelancer { get; set; } // Navigation property to Freelancer

		// Additional fields for Paymob integration
		public string PaymobToken { get; set; } // Tokenized payment method from Paymob
	}
}
