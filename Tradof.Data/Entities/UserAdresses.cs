using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradof.Data.Entities
{
	public class UserAdresses:AuditEntity<long>
	{
		public string user_id { get; set; }
		public string address { get; set; }
	

		[ForeignKey("user_id")]
		public ApplicationUser user { get; set; }
	}
}
