using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasOne(c => c.User)
                      .WithOne()
                      .HasForeignKey<Company>(c => c.UserId);

            builder.HasOne(c => c.Country)
                      .WithMany(co => co.Companies)
                      .HasForeignKey(c => c.CountryId);

            builder.HasMany(c => c.Specializations)
                      .WithMany(s => s.Companies);

            builder.HasMany(c => c.Subscriptions)
                      .WithOne(cs => cs.Company)
                      .HasForeignKey(cs => cs.CompanyId);

            builder.HasMany(c => c.Projects)
                      .WithOne(p => p.Company)
                      .HasForeignKey(p => p.CompanyId);
        }
    }
}
