using Tradof.CountryModule.Services.DTOs;
using Tradof.Data.Entities;

namespace Tradof.CountryModule.Services.Extensions
{
    public static class CountryExtensions
    {
        public static CountryDto ToDto(this Country country)
        {
            return new CountryDto(
                country.Id,
                country.Name
                //country.CreationDate,
                //country.ModificationDate,
                //country.CreatedBy,
                //country.ModifiedBy
            );
        }

        public static void UpdateFromDto(this Country country, UpdateCountryDto dto)
        {
            country.Name = dto.Name;

            if (dto.ModificationDate.HasValue)
            {
                country.ModificationDate = dto.ModificationDate;
            }
            if (!string.IsNullOrEmpty(dto.ModifiedBy))
            {
                country.ModifiedBy = dto.ModifiedBy;
            }
        }
    }
}
