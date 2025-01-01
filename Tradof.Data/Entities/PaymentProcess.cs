using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class PaymentProcess : AuditEntity<long>
    {
		public long PaymentId { get; set; }
		public long FreelancerId { get; set; }
		public long CompanyId { get; set; }

		[ForeignKey("PaymentId")]
		public ProjectPayment Payment { get; set; }

		[ForeignKey("FreelancerId")]
		public Freelancer Freelancer { get; set; }

		[ForeignKey("CompanyId")]
		public Company Company { get; set; }
	}
}