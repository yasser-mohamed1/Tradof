using System.ComponentModel.DataAnnotations;
using Tradof.Common.Exceptions;
using Tradof.Data.Interfaces;
using Tradof.PackageNamespace.Services.DTOs;
using Tradof.PackageNamespace.Services.Extensions;
using Tradof.PackageNamespace.Services.Interfaces;
using Tradof.PackageNamespace.Services.Validation;
using Package = Tradof.Data.Entities.Package;

namespace Tradof.PackageNamespace.Services.Implementation
{
    public class PackageService(IUnitOfWork _unitOfWork) : IPackageService
    {

        public async Task<IReadOnlyList<PackageDto>> GetAllAsync()
        {
            var packages = await _unitOfWork.Repository<Package>().GetAllAsync();
            return packages.Select(p => p.ToDto()).ToList().AsReadOnly();
        }


        public async Task<PackageDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid package ID.");
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id);
            return package == null ? throw new NotFoundException("Package not found") : package.ToDto();
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
            await _unitOfWork.Repository<Package>().AddAsync(package);
            if (await _unitOfWork.CommitAsync())
                return package.ToDto();
            else
                throw new Exception("failed to create");

        }

        public async Task<PackageDto> UpdateAsync(UpdatePackageDto dto)
        {
            ValidationHelper.ValidateUpdatePackageDto(dto);

            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(dto.Id) ?? throw new NotFoundException("Package not found");
            package.UpdateFromDto(dto);

            package.ModificationDate = DateTime.UtcNow;
            package.ModifiedBy = "System";

            await _unitOfWork.Repository<Package>().UpdateAsync(package);

            if (await _unitOfWork.CommitAsync())
                return package.ToDto();
            else
                throw new Exception("failed to update");
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid package ID.");
            var package = await _unitOfWork.Repository<Package>().GetByIdAsync(id) ?? throw new NotFoundException("Package not found");
            await _unitOfWork.Repository<Package>().DeleteAsync(package.Id);

            return await _unitOfWork.CommitAsync();

        }
    }
}