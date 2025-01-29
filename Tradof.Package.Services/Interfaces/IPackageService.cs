using Tradof.PackageNamespace.Services.DTOs;

namespace Tradof.PackageNamespace.Services.Interfaces
{
    public interface IPackageService
    {
        Task<IReadOnlyList<PackageDto>> GetAllAsync();
        Task<PackageDto> GetByIdAsync(long id);
        Task<PackageDto> CreateAsync(CreatePackageDto dto);
        Task<PackageDto> UpdateAsync(UpdatePackageDto dto);
        Task<bool> DeleteAsync(long id);
    }
}