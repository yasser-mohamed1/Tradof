using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.DataBase_Context
{
    public class TradofDbContext(DbContextOptions<TradofDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanySubscription> CompanySubscriptions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Tradof.Data.Entities.File> Files { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }
        public DbSet<FreelancerLanguagesPair> FreelancerLanguagesPairs { get; set; }
        public DbSet<FreelancerSocialMedia> FreelancerSocialMedias { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationReceiver> NotificationReceivers { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProposalAttachments> PropsalAttachments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<TechnicalSupport> TechnicalSupports { get; set; }
        public DbSet<UserAdresses> UserAdresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(u => u.NotificationReceivers)
                      .WithOne(nr => nr.User)
                      .HasForeignKey(nr => nr.UserId);

                entity.HasMany(u => u.Feedbacks)
                      .WithOne(f => f.User)
                      .HasForeignKey(f => f.UserId);

                entity.HasMany(u => u.RatingsFrom)
                      .WithOne(r => r.RaterBy)
                      .HasForeignKey(r => r.RatedById)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.RatingsTo)
                      .WithOne(r => r.RaterTo)
                      .HasForeignKey(r => r.RatedToId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasOne(c => c.User)
                      .WithOne()
                      .HasForeignKey<Company>(c => c.UserId);

                entity.HasOne(c => c.Country)
                      .WithMany(co => co.Companies)
                      .HasForeignKey(c => c.CountryId);

                entity.HasOne(c => c.Specialization)
                      .WithMany(s => s.Companies)
                      .HasForeignKey(c => c.SpecializationId);

                entity.HasMany(c => c.Subscriptions)
                      .WithOne(cs => cs.Company)
                      .HasForeignKey(cs => cs.CompanyId);

                entity.HasMany(c => c.Projects)
                      .WithOne(p => p.Company)
                      .HasForeignKey(p => p.CompanyId);
            });

            // Freelancer
            modelBuilder.Entity<Freelancer>(entity =>
            {
                entity.HasOne(f => f.User)
                      .WithOne()
                      .HasForeignKey<Freelancer>(f => f.UserId);

                entity.HasOne(f => f.Specialization)
                      .WithMany(s => s.Freelancers)
                      .HasForeignKey(f => f.SpecializationId);

                entity.HasMany(f => f.Projects)
                      .WithOne(p => p.Freelancer)
                      .HasForeignKey(p => p.FreelancerId);

                entity.HasMany(f => f.FreelancerLanguagesPairs)
                      .WithOne(fl => fl.Freelancer)
                      .HasForeignKey(fl => fl.FreelancerId);

                entity.HasMany(f => f.Proposals)
                      .WithOne(pr => pr.Freelancer)
                      .HasForeignKey(pr => pr.FreelancerId);
            });

            // Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasOne(p => p.Company)
                      .WithMany(c => c.Projects)
                      .HasForeignKey(p => p.CompanyId);

                entity.HasOne(p => p.Freelancer)
                      .WithMany(f => f.Projects)
                      .HasForeignKey(p => p.FreelancerId);

                entity.HasOne(p => p.LanguageFrom)
                      .WithMany(l => l.ProjectsLanguageFrom)
                      .HasForeignKey(p => p.LanguageFromId);

                entity.HasOne(p => p.LanguageTo)
                      .WithMany(l => l.ProjectsLanguageTo)
                      .HasForeignKey(p => p.LanguageToId);

                entity.HasMany(p => p.Files)
                      .WithOne(f => f.Project)
                      .HasForeignKey(f => f.ProjectId);

                entity.HasMany(p => p.Ratings)
                      .WithOne(r => r.Project)
                      .HasForeignKey(r => r.ProjectId);

                entity.HasMany(p => p.Proposals)
                      .WithOne(pr => pr.Project)
                      .HasForeignKey(pr => pr.ProjectId);
            });

            modelBuilder.Entity<FreelancerLanguagesPair>()
                .HasOne(flp => flp.LanguageFrom)
                .WithMany(l => l.LanguagePairsFrom)
                .HasForeignKey(flp => flp.LanguageFromId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FreelancerLanguagesPair>()
                .HasOne(flp => flp.LanguageTo)
                .WithMany(l => l.LanguagePairsTo)
                .HasForeignKey(flp => flp.LanguageToId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorksOn
            modelBuilder.Entity<WorksOn>(entity =>
            {
                entity.HasOne(wo => wo.Freelancer)
                      .WithMany(f => f.WorksOns)
                      .HasForeignKey(wo => wo.FreelancerId);

                entity.HasOne(wo => wo.Project)
                      .WithOne();
            });

            modelBuilder.Entity<SupportTicket>()
               .HasOne(s => s.TechnicalSupport)
               .WithMany(t => t.SupportTickets)
               .HasForeignKey(s => s.SupporterId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Freelancer)
                .WithMany(f => f.Projects)
                .HasForeignKey(p => p.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.LanguageTo)
                .WithMany()
                .HasForeignKey(p => p.LanguageToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Specialization)
                .WithMany(s => s.Projects)
                .HasForeignKey(p => p.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Project)
                .WithMany(project => project.Proposals)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorksOn>()
                .HasOne(w => w.Project)
                .WithMany()
                .HasForeignKey(w => w.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Project)
                .WithMany()
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentProcess>()
                .HasOne(pp => pp.Freelancer)
                .WithMany()
                .HasForeignKey(pp => pp.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentProcess>()
            .HasOne(pp => pp.Payment)
            .WithMany()
            .HasForeignKey(pp => pp.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectPayment>()
                .HasOne(pp => pp.Project)
                .WithMany()
                .HasForeignKey(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

    }
}