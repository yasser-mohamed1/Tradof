using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.DataBase_Context
{
    public class TradofDbContext : IdentityDbContext
    {
        public TradofDbContext(DbContextOptions<TradofDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<ApplicationUser> Users { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanySubscription> CompanySubscriptions { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Tradof.Data.Entities.File> Files { get; set; }
        public virtual DbSet<Freelancer> Freelancers { get; set; }
        public virtual DbSet<FreelancerLanguagesPair> FreelancerLanguagesPairs { get; set; }
        public virtual DbSet<FreelancerSocialMedia> FreelancerSocialMedias { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationReceiver> NotificationReceivers { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Proposal> Proposals { get; set; }
        public virtual DbSet<ProposalAttachments> PropsalAttachments { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<SupportTicket> SupportTickets { get; set; }
        public virtual DbSet<TechnicalSupport> TechnicalSupports { get; set; }
        public virtual DbSet<UserAdresses> UserAdresses { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
    }
}