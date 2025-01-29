using Tradof.CountryModule.Services.DTOs;

namespace Tradof.CountryModule.Services.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDto>> GetAllAsync();
        Task<CountryDto> GetByIdAsync(long id);
        Task<CountryDto> CreateAsync(CreateCountryDto dto);
        Task<CountryDto> UpdateAsync(UpdateCountryDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
