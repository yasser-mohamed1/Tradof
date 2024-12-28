using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class PojectPayment: AuditEntity<long>
    {
		
		public string TransactionNumber { get; set; }	
		public double amount { get; set; }
		public PaymentStatus payment_status { get; set; }
		public PaymentMethod payment_method { get; set; }
		public DateTime paymeny_date { get; set; }
		public string project_id { get; set; }
	

		[ForeignKey("project_id")]
		public Project Project { get; set; }
		public virtual PaymentProcess PaymentProcess { get; set; } = new PaymentProcess();


	}
}
