using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class Company
	{
		public string company_id { get; set; }
		public string company_address { get; set; }
        public string job_title {  get; set; }
		public string? specialization {  get; set; }
		public string country { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public CompanyUserType company_user_type { get; set; }

		[ForeignKey("comapny_id")]
		public ApplicationUser User { get; set; }
		
		public virtual ICollection<CompanySubscription> Subscribtions { get; set; } = new List<CompanySubscription>();
		public virtual ICollection<Project> Projects { get; set; } = new List<Project>();


	}
}
