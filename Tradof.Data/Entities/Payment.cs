using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Payment
	{
		public string payment_id { get; set; }
		public double amount { get; set; }
		public PaymentStatus payment_status { get; set; }
		public PaymentMethod payment_method { get; set; }
		public DateTime paymeny_date { get; set; }
		public string project_id { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[ForeignKey("project_id")]
		public Project Project { get; set; }
		public virtual PaymentProcess PaymentProcess { get; set; } = new PaymentProcess();


	}
}
