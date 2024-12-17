using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Invoice
	{
		public string invoice_id { get; set; }
		public int inoveice_number { get; set; }
		public double total_amount { get; set; }
		public InvoiceStatus invoice_status { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public string project_id { get; set; }
		public string payment_id { get; set; }

		[ForeignKey("project_id")]
		public Project Project { get; set; }
		[ForeignKey("payment_id")]
		public Payment Payment { get; set; }
	}
}
