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
        }
    }
}
