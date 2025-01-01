using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
	public class UserAdresses : AuditEntity<long>
	{
		public string Address { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
	}
}