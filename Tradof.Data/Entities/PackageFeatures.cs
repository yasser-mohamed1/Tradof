using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class PackageFeatures
	{
		public string package_id { get; set; }
		public string feature { get; set; }
		[ForeignKey("package_id")]
		public Package Package { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
	}
}
