using System.ComponentModel.DataAnnotations;
using Tradof.Common.Exceptions;
using Tradof.Data.Interfaces;
using Tradof.Language.Services.DTOs;
using Tradof.Language.Services.Extensions;
using Tradof.Language.Services.Interfaces;
using LanguageEntity = Tradof.Data.Entities.Language;

namespace Tradof.Language.Services.Implementation
{
    public class LanguageService(IGeneralRepository<LanguageEntity> _repository) : ILanguageService
    {
        public async Task<IEnumerable<LanguageDto>> GetAllAsync()
        {
            var languages = await _repository.GetAllAsync();
            return languages.Select(l => l.ToDto());
        }

        public async Task<LanguageDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid language ID.");
            var language = await _repository.GetByIdAsync(id);
            return language == null ? throw new NotFoundException("Language not found") : language.ToDto();
        }

        public async Task<LanguageDto> CreateAsync(CreateLanguageDto dto)
        {

            var language = new LanguageEntity
            {
                Name = dto.Name,
                Code = dto.Code,
                CreatedBy = "System",
                CreationDate = DateTime.UtcNow,
                ModifiedBy = "System"
            };
            await _repository.AddAsync(language);
            return language.ToDto();
        }

        public async Task<LanguageDto> UpdateAsync(UpdateLanguageDto dto)
        {

            var language = await _repository.GetByIdAsync(dto.Id) ?? throw new NotFoundException("Language not found");
            language.UpdateFromDto(dto);

            language.ModificationDate = DateTime.UtcNow;
            language.ModifiedBy = "System";

            await _repository.UpdateAsync(language);
            return language.ToDto();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid language ID.");
            var language = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Language not found");
            await _repository.DeleteAsync(language.Id);
            return true;
        }
    }
}
