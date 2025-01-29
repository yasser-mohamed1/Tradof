using Tradof.PackageNamespace.Services.DTOs;
using PackageEntity = Tradof.Data.Entities.Package;

namespace Tradof.PackageNamespace.Services.Extensions
{
    public static class PackageExtensions
    {
        public static PackageDto ToDto(this PackageEntity package)
        {
            return new PackageDto(
                package.Id,
                package.Name,
                package.Description,
                package.Price,
                package.DurationInMonths,
                package.CreationDate,
                package.ModificationDate,
                package.CreatedBy,
                package.ModifiedBy
            );
        }

        public static void UpdateFromDto(this PackageEntity package, UpdatePackageDto dto)
        {
            package.Name = dto.Name;
            package.Description = dto.Description;
            package.Price = dto.Price;
            package.DurationInMonths = dto.DurationInMonths;

            if (dto.ModificationDate.HasValue)
            {
                package.ModificationDate = dto.ModificationDate;
            }
            if (!string.IsNullOrEmpty(dto.ModifiedBy))
            {
                package.ModifiedBy = dto.ModifiedBy;
            }
        }
    }

}