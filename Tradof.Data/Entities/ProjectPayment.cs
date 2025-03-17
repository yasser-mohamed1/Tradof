using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
	public class ProjectPayment : AuditEntity<long>
	{
		public string TransactionNumber { get; set; } // Paymob transaction ID
		public double Amount { get; set; }
		public PaymentStatus PaymentStatus { get; set; }
		public PaymentMethodEnum PaymentMethod { get; set; }
		public DateTime PaymentDate { get; set; }
		public long ProjectId { get; set; } // Foreign key to Project

		[ForeignKey("ProjectId")]
		public Project Project { get; set; } // Navigation property to Project

		public PaymentProcess PaymentProcess { get; set; } // Navigation property to PaymentProcess

		// Additional fields for Paymob integration
		public string PaymobOrderId { get; set; } // Paymob order ID
		public string PaymobPaymentKey { get; set; } // Paymob payment key
		public string PaymobTransactionId { get; set; } // Paymob transaction ID
		public string PaymobResponse { get; set; } // Raw response from Paymob (for debugging)
	}
}