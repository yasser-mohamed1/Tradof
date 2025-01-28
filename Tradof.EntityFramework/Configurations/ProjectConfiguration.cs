using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {

            builder.HasOne(p => p.Company)
                    .WithMany(c => c.Projects)
                    .HasForeignKey(p => p.CompanyId);

            builder.HasOne(p => p.Freelancer)
                .WithMany(f => f.Projects)
                .HasForeignKey(p => p.FreelancerId);

            builder.HasOne(p => p.LanguageFrom)
                .WithMany(l => l.ProjectsLanguageFrom)
                .HasForeignKey(p => p.LanguageFromId);

            builder.HasOne(p => p.LanguageTo)
                .WithMany(l => l.ProjectsLanguageTo)
                .HasForeignKey(p => p.LanguageToId);

            builder.HasMany(p => p.Files)
                .WithOne(f => f.Project)
                .HasForeignKey(f => f.ProjectId);

            builder.HasMany(p => p.Ratings)
                .WithOne(r => r.Project)
                .HasForeignKey(r => r.ProjectId);

            builder.HasMany(p => p.Proposals)
                .WithOne(pr => pr.Project)
                .HasForeignKey(pr => pr.ProjectId);

            builder.HasOne(p => p.Freelancer)
                .WithMany(f => f.Projects)
                .HasForeignKey(p => p.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.LanguageTo)
                .WithMany()
                .HasForeignKey(p => p.LanguageToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Specialization)
                .WithMany(s => s.Projects)
                .HasForeignKey(p => p.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
