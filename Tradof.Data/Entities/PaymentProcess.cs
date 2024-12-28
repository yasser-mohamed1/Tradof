using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class PaymentProcess: AuditEntity<long>
    {
		public string payment_id { get; set; }
		public string freelancer_id { get; set; }
		public string company_id { get; set; }
		public DateTime payment_date { get; set; }
		

		[ForeignKey("payment_id")]
		public Payment Payment { get; set; }

		[ForeignKey("freelancer_id")]
		public Freelancer Freelancer { get; set; }

		[ForeignKey("company_id")]
		public Company Company { get; set; }
	}
}
