using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder.HasMany(u => u.NotificationReceivers)
                  .WithOne(nr => nr.User)
                  .HasForeignKey(nr => nr.UserId);

            builder.HasMany(u => u.Feedbacks)
                  .WithOne(f => f.User)
                  .HasForeignKey(f => f.UserId);

            builder.HasMany(u => u.RatingsFrom)
                  .WithOne(r => r.RaterBy)
                  .HasForeignKey(r => r.RatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.RatingsTo)
                  .WithOne(r => r.RaterTo)
                  .HasForeignKey(r => r.RatedToId)
                  .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
