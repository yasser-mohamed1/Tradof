using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class Package
	{
		public string package_id { get; set; } = Guid.NewGuid().ToString();
		public string package_name {  get; set; }
		public int durtation_months { get; set; }
		public double price { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
	}
}
