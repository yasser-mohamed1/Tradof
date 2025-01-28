using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.HasOne(s => s.TechnicalSupport)
                .WithMany(t => t.SupportTickets)
                .HasForeignKey(s => s.SupporterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
