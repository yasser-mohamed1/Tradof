using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class PaymentProcess : AuditEntity<long>
	{
		public long PaymentId { get; set; } // Foreign key to ProjectPayment
		public long FreelancerId { get; set; } // Foreign key to Freelancer
		public long CompanyId { get; set; } // Foreign key to Company

		[ForeignKey("PaymentId")]
		public ProjectPayment Payment { get; set; } // Navigation property to ProjectPayment

		[ForeignKey("FreelancerId")]
		public Freelancer Freelancer { get; set; } // Navigation property to Freelancer

		[ForeignKey("CompanyId")]
		public Company Company { get; set; } // Navigation property to Company
	}
}