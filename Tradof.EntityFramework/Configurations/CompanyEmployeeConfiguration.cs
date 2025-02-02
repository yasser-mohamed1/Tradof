using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class CompanyEmployeeConfiguration : IEntityTypeConfiguration<CompanyEmployee>
    {
        public void Configure(EntityTypeBuilder<CompanyEmployee> builder)
        {
            builder.HasOne(ce => ce.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(ce => ce.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
