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
	public class Company: AuditEntity<T>
	{
		
		public string CompanyAddress { get; set; }
        public string JobTitle {  get; set; }
		public string? Specialization {  get; set; }
		public string Country { get; set; }
	
		public CompanyUserType UserType { get; set; }

		[ForeignKey("comapny_id")]
		public ApplicationUser User { get; set; }
		
		public virtual ICollection<CompanySubscription> Subscribtions { get; set; } = new List<CompanySubscription>();
		public virtual ICollection<Project> Projects { get; set; } = new List<Project>();


	}
}
