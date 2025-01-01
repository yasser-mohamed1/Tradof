using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class PaymentProcess : AuditEntity<long>
    {
		public string PaymentId { get; set; }
		public string FreelancerId { get; set; }
		public string CompanyId { get; set; }

		[ForeignKey("PaymentId")]
		public ProjectPayment Payment { get; set; }

		[ForeignKey("FreelancerId")]
		public Freelancer Freelancer { get; set; }

		[ForeignKey("CompanyId")]
		public Company Company { get; set; }
	}
}