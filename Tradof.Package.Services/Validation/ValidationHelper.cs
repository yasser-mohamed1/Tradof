using System.ComponentModel.DataAnnotations;
using Tradof.PackageNamespace.Services.DTOs;

namespace Tradof.PackageNamespace.Services.Validation
{
    public static class ValidationHelper
    {
        public static void ValidateCreatePackageDto(CreatePackageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ValidationException("Description cannot be empty.");
            if (dto.Price <= 0)
                throw new ValidationException("Price must be greater than 0.");
            if (dto.DurationInMonths <= 0)
                throw new ValidationException("Duration must be greater than 0.");
        }

        public static void ValidateUpdatePackageDto(UpdatePackageDto dto)
        {
            if (dto.Id <= 0)
                throw new ValidationException("Invalid package ID.");
            ValidateCreatePackageDto(new CreatePackageDto(dto.Name, dto.Description, dto.Price, dto.DurationInMonths));
        }
    }
}