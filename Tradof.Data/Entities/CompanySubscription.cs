using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class CompanySubscription : AuditEntity<T>
    {
		public string company_id { get; set; }
		public string package_id { get; set; }
		public DateTime start_date { get; set; }
		public DateTime end_date { get; set; }
		public string? coupon { get; set; }
		public double net_price { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[ForeignKey("company_id")]
		public Company Company { get; set; }
		[ForeignKey("package_id")]
		public Package Package { get; set; }

	}
}
