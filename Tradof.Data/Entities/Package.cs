using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class Package :AuditEntity<T>
	{
	
		public string package_name {  get; set; }
		public int durtation_months { get; set; }
		public double price { get; set; }
	
	}
}
