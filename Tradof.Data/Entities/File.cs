﻿using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class File : AuditEntity<long>
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public FileType FileType { get; set; }
        public long FileSize { get; set; }
        public long ProjectId { get; set; }
        public string PublicId { get; set; }
        public bool IsFreelancerUpload { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}