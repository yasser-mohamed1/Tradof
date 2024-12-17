using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradof.Comman.Enums;

namespace Tradof.Data.Entities
{
	public class File
	{
		public string file_id { get; set; }
		public string file_name { get; set; }
		public string file_path { get; set; }
		public FileType file_type { get; set; }
		public int file_size { get; set; }	
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public string project_id { get; set; }

		[ForeignKey("project_id")]
		public Project Project { get; set; }

	}
}
