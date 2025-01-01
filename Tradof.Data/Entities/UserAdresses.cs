using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class UserAdresses : AuditEntity<long>
	{
		public string Address { get; set; }

		[ForeignKey("Id")]
		public ApplicationUser User { get; set; }
	}
}