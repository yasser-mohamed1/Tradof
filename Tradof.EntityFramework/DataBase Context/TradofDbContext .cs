using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tradof.Data.Entities;
using Tradof.EntityFramework.Configurations;

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
            #region appUser
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);
            #endregion

            #region Company
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyConfiguration).Assembly);
            #endregion

            #region Freelancer
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FreelancerConfiguration).Assembly);
            #endregion

            #region project
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectConfiguration).Assembly);
            #endregion

            #region WorksOn
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorksOnConfiguration).Assembly);
            #endregion

            #region FreelancerLanguagesPair
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FreelancerLanguagesPairConfiguration).Assembly);
            #endregion

            #region SupportTicket
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportTicketConfiguration).Assembly);
            #endregion

            #region Proposal
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProposalConfiguration).Assembly);
            #endregion

            #region Invoice
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceConfiguration).Assembly);
            #endregion

            #region PaymentProcess
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentProcessConfiguration).Assembly);
            #endregion

            #region ProjectPayment
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectPaymentConfiguration).Assembly);
            #endregion    

            base.OnModelCreating(modelBuilder);
        }

    }
}