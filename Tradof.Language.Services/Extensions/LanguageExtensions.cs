using Tradof.Language.Services.DTOs;
using LanguageEntity = Tradof.Data.Entities.Language;

namespace Tradof.Language.Services.Extensions
{
    public static class LanguageExtensions
    {
        public static LanguageDto ToDto(this LanguageEntity language)
        {
            return new LanguageDto(
                language.Id,
                language.LanguageName ?? "N/A",
                language.LanguageCode ?? "N/A",
				language.CountryName ?? "N/A",
				language.CountryCode ?? "N/A"
			);
        }

        public static void UpdateFromDto(this LanguageEntity language, UpdateLanguageDto dto)
        {
            language.LanguageName = dto.LanguageName;
            language.LanguageCode = dto.LanguageCode;
			language.CountryName = dto.CountryName;
			language.CountryCode = dto.CountryCode;

			if (dto.ModificationDate.HasValue)
            {
                language.ModificationDate = dto.ModificationDate;
            }
            if (!string.IsNullOrEmpty(dto.ModifiedBy))
            {
                language.ModifiedBy = dto.ModifiedBy;
            }
        }
    }
}
