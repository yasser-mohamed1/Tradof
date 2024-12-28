using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class FreelancerLanguagesPair : AuditEntity<long>
    {
		public string freelancer_id { get; set; }
		public string language_from { get; set; }
		public string language_to { get; set; }


		[ForeignKey("freelancer_id")]
		public Freelancer Freelancer { get; set; }
	}
}
