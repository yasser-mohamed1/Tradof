﻿using Tradof.Common.Enums;
using Tradof.Data.Entities;

namespace Tradof.Data.Specifications
{
    public class StartedProjectsByCompanySpecification : BaseSpecification<Project>
    {
        public StartedProjectsByCompanySpecification(long companyId)
            : base(p => p.Status == ProjectStatus.InProgress && p.CompanyId == companyId)
        {
            AddInclude(p => p.Files);
            AddInclude(p => p.Specialization);
            AddInclude(p => p.LanguageFrom);
            AddInclude(p => p.LanguageTo);
            AddInclude(p => p.Company);
            AddInclude(p => p.Company.User);
            AddInclude(p => p.Proposals);
            AddInclude(p => p.Freelancer);
            AddInclude(p => p.Freelancer.User);

        }
    }
}
