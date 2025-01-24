using Tradof.Package.Services.DTOs;

namespace Tradof.Package.Services.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageDto>> GetAllAsync();
        Task<PackageDto> GetByIdAsync(long id);
        Task<PackageDto> CreateAsync(CreatePackageDto dto);
        Task<PackageDto> UpdateAsync(UpdatePackageDto dto);
        Task<bool> DeleteAsync(long id);
    }
}