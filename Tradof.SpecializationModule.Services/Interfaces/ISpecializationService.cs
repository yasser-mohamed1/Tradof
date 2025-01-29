using Tradof.SpecializationModule.Services.DTOs;

namespace Tradof.SpecializationModule.Services.Interfaces
{
    public interface ISpecializationService
    {
        Task<IEnumerable<SpecializationDto>> GetAllAsync();
        Task<SpecializationDto> GetByIdAsync(long id);
        Task<SpecializationDto> CreateAsync(CreateSpecializationDto dto);
        Task<SpecializationDto> UpdateAsync(UpdateSpecializationDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
