﻿
using Tradof.Data.Entities;
using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Extensions
{
    public static class ProjectExtensions
    {
        public static ProjectDto ToDto(this Data.Entities.Project project)
        {
            var specialization = project.Specialization == null
            ? null
            : new SpecializationDto(
                project.SpecializationId ?? 0,
                project.Specialization.Name
            );

            var languageFrom = new LanguageDto(
                project.LanguageFromId,
                project.LanguageFrom.LanguageName,
                project.LanguageFrom.LanguageCode,
                project.LanguageFrom.CountryName,
                project.LanguageFrom.CountryCode
            );

            var languageTo = new LanguageDto(

                project.LanguageToId,
                project.LanguageTo.LanguageName,
                project.LanguageTo.LanguageCode,
                project.LanguageTo.CountryName,
                project.LanguageTo.CountryCode
            );

            var status = new ProjectStatusDto(
                (int)project.Status,
                project.Status.ToString()
            );

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Days = project.Days,
                Description = project.Description,
                LanguageFrom = languageFrom,
                LanguageTo = languageTo,
                MaxPrice = project.MaxPrice,
                MinPrice = project.MinPrice,
                NumberOfOffers = project.Proposals.Count,
                Specialization = specialization,
                Files = project.Files.Select(f => f.ToDto()).ToList(),
                Price = project.Price,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = status,
                DeliveryDate = project.DeliveryDate,
                CompanyId = project.Company.UserId,
                FirstName = project.Freelancer != null ? project.Freelancer.User.FirstName : string.Empty,
                LastName = project.Freelancer != null ? project.Freelancer.User.LastName : string.Empty,
                JobTitle = project.Company.JobTitle,
                ProfileImageUrl = project.Company.User.ProfileImageUrl,
                CreationDate = project.CreationDate,
                FreelancerId = project.Freelancer != null ? project.Freelancer.UserId : string.Empty,
                FreelancerEmail = project.Freelancer != null ? project.Freelancer.User.Email : string.Empty,
                CancellationAccepted = project.CancellationAccepted,
                CancellationAcceptedBy = project.CancellationAcceptedBy,
                CancellationAcceptedDate = project.CancellationAcceptedDate,
                CancellationRequested = project.CancellationRequested,
                CancellationRequestedBy = project.CancellationRequestedBy,
                CancellationRequestDate = project.CancellationRequestDate,
                CancellationResponse = project.CancellationResponse,
                ProposalId = project.AcceptedProposalId != null ? project.AcceptedProposalId : null,
                RatingFromFreelancer = project.Ratings.FirstOrDefault(r => r.RatedById == project.Freelancer.UserId)?.ToDto(),
                RatingFromCompany = project.Ratings.FirstOrDefault(r => r.RatedById == project.Company.UserId)?.ToDto()
            };
        }

        public static UnassignedProjecstDto ToUnassignedProjectDto(this Data.Entities.Project project, long? freelancerId)
        {
            var specialization = project.Specialization == null
            ? null
            : new SpecializationDto(
                project.SpecializationId ?? 0,
                project.Specialization.Name
            );

            var languageFrom = new LanguageDto(
                project.LanguageFromId,
                project.LanguageFrom.LanguageName,
                project.LanguageFrom.LanguageCode,
                project.LanguageFrom.CountryName,
                project.LanguageFrom.CountryCode
            );

            var languageTo = new LanguageDto(

                project.LanguageToId,
                project.LanguageTo.LanguageName,
                project.LanguageTo.LanguageCode,
                project.LanguageTo.CountryName,
                project.LanguageTo.CountryCode
            );

            var status = new ProjectStatusDto(
                (int)project.Status,
                project.Status.ToString()
            );

            return new UnassignedProjecstDto
            {
                Id = project.Id,
                Name = project.Name,
                Days = project.Days,
                Description = project.Description,
                LanguageFrom = languageFrom,
                LanguageTo = languageTo,
                MaxPrice = project.MaxPrice,
                MinPrice = project.MinPrice,
                NumberOfOffers = project.Proposals.Count,
                Specialization = specialization,
                Files = project.Files.Select(f => f.ToDto()).ToList(),
                Price = project.Price,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = status,
                DeliveryDate = project.DeliveryDate,
                CompanyId = project.Company.UserId,
                FirstName = project.Company != null ? project.Company.User.FirstName : string.Empty,
                LastName = project.Company != null ? project.Company.User.LastName : string.Empty,
                JobTitle = project.Company.JobTitle,
                ProfileImageUrl = project.Company.User.ProfileImageUrl,
                CreationDate = project.CreationDate,
                FreelancerId = project.Freelancer != null ? project.Freelancer.UserId : string.Empty,
                FreelancerEmail = project.Freelancer != null ? project.Freelancer.User.Email : string.Empty,
                CancellationAccepted = project.CancellationAccepted,
                CancellationAcceptedBy = project.CancellationAcceptedBy,
                CancellationAcceptedDate = project.CancellationAcceptedDate,
                CancellationRequested = project.CancellationRequested,
                CancellationRequestedBy = project.CancellationRequestedBy,
                CancellationRequestDate = project.CancellationRequestDate,
                CancellationResponse = project.CancellationResponse,
                ProposalId = project.AcceptedProposalId != null ? project.AcceptedProposalId : null,
                RatingFromFreelancer = project.Ratings.FirstOrDefault(r => r.RatedById == project.Freelancer.UserId)?.ToDto(),
                RatingFromCompany = project.Ratings.FirstOrDefault(r => r.RatedById == project.Company.UserId)?.ToDto(),
                Applied = freelancerId != null ? project.Proposals.Any(p => p.FreelancerId == freelancerId) : false
            };
        }

        public static RatingDto ToDto(this Rating rating)
        {
            if (rating == null)
                return null;

            return new RatingDto
            {
                Id = rating.Id,
                RatingValue = rating.RatingValue,
                Review = rating.Review,
                ProjectId = rating.ProjectId,
                RatedToId = rating.RatedToId,
                RatedById = rating.RatedById,
                RatedByName = rating.RaterBy != null
                    ? $"{rating.RaterBy.FirstName} {rating.RaterBy.LastName}".Trim()
                    : string.Empty,
                RatedToName = rating.RaterTo != null
                    ? $"{rating.RaterTo.FirstName} {rating.RaterTo.LastName}".Trim()
                    : string.Empty,
                CreationDate = rating.CreationDate
            };
        }

        public static Data.Entities.Project ToEntity(this CreateProjectDto projectDto)
        {
            return new Data.Entities.Project
            {
                MinPrice = projectDto.MinPrice,
                Days = projectDto.Days,
                Description = projectDto.Description,
                Name = projectDto.Name,
                MaxPrice = projectDto.MaxPrice,
                Status = Common.Enums.ProjectStatus.Pending,
            };
        }

        public static void UpdateFromDto(this Data.Entities.Project project, UpdateProjectDto projectDto)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            if (projectDto == null) throw new ArgumentNullException(nameof(projectDto));
            project.MinPrice = projectDto.MinPrice;
            project.Days = projectDto.Days;
            project.Description = projectDto.Description;
            project.Name = projectDto.Name;
            project.MaxPrice = projectDto.MaxPrice;

        }

        public static ProjectCardDto ToProjectCardDto(this Data.Entities.Project project)
        {
            string ownerName = project.Company.User.FirstName + " " + project.Company.User.LastName;
            string ownerEmail = project.Company.User.Email ?? "N/A";
            string companyName = project.Company.CompanyName ?? "N/A";
            var budget = new Budget
            {
                MinPrice = project.MinPrice,
                MaxPrice = project.MaxPrice
            };
            return new ProjectCardDto
            {
                ProjectState = project.Status.ToString(),
                ProjectStartDate = project.StartDate,
                Budget = budget,
                Duration = project.Days,
                NumberOfOffers = project.Proposals.Count,
                OwnerName = ownerName,
                OwnerEmail = ownerEmail,
                CompanyName = companyName,
                projectOwnerImage = project.Company.User.ProfileImageUrl,
                RegisteredAt = project.Company.CreationDate,
                TotalProjects = project.Company.Projects.Count,
                OpenProjects = project.Company.Projects.Count(p => p.Status != Common.Enums.ProjectStatus.Finished)
            };
        }
    }
}