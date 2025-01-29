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
                language.Name,
                language.Code
                //language.CreationDate,
                //language.ModificationDate,
                //language.CreatedBy,
                //language.ModifiedBy
            );
        }

        public static void UpdateFromDto(this LanguageEntity language, UpdateLanguageDto dto)
        {
            language.Name = dto.Name;
            language.Code = dto.Code;

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
