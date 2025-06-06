using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class FreelancerLanguagesPairConfiguration : IEntityTypeConfiguration<FreelancerLanguagesPair>
    {
        public void Configure(EntityTypeBuilder<FreelancerLanguagesPair> builder)
        {

            builder
                .HasOne(flp => flp.LanguageFrom)
                .WithMany(l => l.LanguagePairsFrom)
                .HasForeignKey(flp => flp.LanguageFromId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(flp => flp.LanguageTo)
                .WithMany(l => l.LanguagePairsTo)
                .HasForeignKey(flp => flp.LanguageToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(f => f.Free, nav =>
            {
                nav.Property(e => e.HasTakenExam).HasColumnName("Free_HasTakenExam");
                nav.Property(e => e.Mark).HasColumnName("Free_Mark");
                nav.WithOwner();
            }).Navigation(e => e.Free).IsRequired(false);

            builder.OwnsOne(f => f.Pro1, nav =>
            {
                nav.Property(e => e.HasTakenExam).HasColumnName("Pro1_HasTakenExam");
                nav.Property(e => e.Mark).HasColumnName("Pro1_Mark");
                nav.WithOwner();
            }).Navigation(e => e.Pro1).IsRequired(false);

            builder.OwnsOne(f => f.Pro2, nav =>
            {
                nav.Property(e => e.HasTakenExam).HasColumnName("Pro2_HasTakenExam");
                nav.Property(e => e.Mark).HasColumnName("Pro2_Mark");
                nav.WithOwner();
            }).Navigation(e => e.Pro2).IsRequired(false);
        }
    }
}
