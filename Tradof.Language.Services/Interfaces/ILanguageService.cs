using Tradof.Language.Services.DTOs;

namespace Tradof.Language.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<IEnumerable<LanguageDto>> GetAllAsync();
        Task<LanguageDto> GetByIdAsync(long id);
        Task<LanguageDto> CreateAsync(CreateLanguageDto dto);
        Task<LanguageDto> UpdateAsync(UpdateLanguageDto dto);
        Task<bool> DeleteAsync(long id);
    }
}
