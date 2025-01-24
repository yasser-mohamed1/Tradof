using System.ComponentModel.DataAnnotations;
using Tradof.Common.Exceptions;
using Tradof.Data.Interfaces;
using Tradof.Package.Services.DTOs;
using Tradof.Package.Services.Extensions;
using Tradof.Package.Services.Interfaces;
using Tradof.Package.Services.Validation;

namespace Tradof.Package.Services.Implementation
{
    public class PackageService : IPackageService
    {
        private readonly IGeneralRepository<Tradof.Data.Entities.Package> _repository;

        public PackageService(IGeneralRepository<Tradof.Data.Entities.Package> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PackageDto>> GetAllAsync()
        {
            var packages = await _repository.GetAllAsync();
            return packages.Select(p => p.ToDto());
        }

        public async Task<PackageDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid package ID.");
            var package = await _repository.GetByIdAsync(id);
            if (package == null) throw new NotFoundException("Package not found");
            return package.ToDto();
        }

        public async Task<PackageDto> CreateAsync(CreatePackageDto dto)
        {
            ValidationHelper.ValidateCreatePackageDto(dto);

            var package = new Tradof.Data.Entities.Package
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationInMonths = dto.DurationInMonths,
                CreationDate = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System"
            };
            await _repository.AddAsync(package);
            return package.ToDto();
        }

        public async Task<PackageDto> UpdateAsync(UpdatePackageDto dto)
        {
            ValidationHelper.ValidateUpdatePackageDto(dto);

            var package = await _repository.GetByIdAsync(dto.Id);
            if (package == null) throw new NotFoundException("Package not found");

            package.UpdateFromDto(dto);

            package.ModificationDate = DateTime.UtcNow;
            package.ModifiedBy = "System";

            await _repository.UpdateAsync(package);
            return package.ToDto();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid package ID.");
            var package = await _repository.GetByIdAsync(id);
            if (package == null) throw new NotFoundException("Package not found");
            await _repository.DeleteAsync(package.Id);
            return true;
        }
    }
}