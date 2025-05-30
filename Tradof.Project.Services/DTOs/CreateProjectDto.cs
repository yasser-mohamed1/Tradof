﻿using Microsoft.AspNetCore.Http;

namespace Tradof.Project.Services.DTOs
{
    public class CreateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Days { get; set; }
        public long LanguageFromId { get; set; }
        public long LanguageToId { get; set; }
		public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public long SpecializationId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
