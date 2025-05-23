﻿using Tradof.Common.Enums;

namespace Tradof.Project.Services.DTOs
{
    public class FileDto
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public FileType FileType { get; set; }
        public long FileSize { get; set; }
        public long ProjectId { get; set; }
        public bool IsFreelancerUpload { get; set; }
    }
}
