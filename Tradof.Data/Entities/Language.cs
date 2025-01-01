﻿using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class Language : AuditEntity<long>
    {
        public string Name { get; set; }

        public virtual ICollection<Project> ProjectsLanguageTo { get; set; } = new List<Project>();
        public virtual ICollection<Project> ProjectsLanguageFrom { get; set; } = new List<Project>();
        public virtual ICollection<FreelancerLanguagesPair> LanguagePairsTo { get; set; } = new List<FreelancerLanguagesPair>();
        public virtual ICollection<FreelancerLanguagesPair> LanguagePairsFrom { get; set; } = new List<FreelancerLanguagesPair>();
    }
}
