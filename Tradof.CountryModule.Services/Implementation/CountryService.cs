using System.ComponentModel.DataAnnotations;
using Tradof.Common.Exceptions;
using Tradof.CountryModule.Services.DTOs;
using Tradof.CountryModule.Services.Extensions;
using Tradof.CountryModule.Services.Interfaces;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;

namespace Tradof.CountryModule.Services.Implementation
{
    public class CountryService(IGeneralRepository<Country> _repository) : ICountryService
    {
        public async Task<IEnumerable<CountryDto>> GetAllAsync()
        {
            var countries = await _repository.GetAllAsync();
            return countries.Select(c => c.ToDto());
        }

        public async Task<CountryDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid country ID.");
            var country = await _repository.GetByIdAsync(id);
            return country == null ? throw new NotFoundException("Country not found") : country.ToDto();
        }

        public async Task<CountryDto> CreateAsync(CreateCountryDto dto)
        {
            var country = new Country
            {
                Name = dto.Name,
                CreatedBy = "System",
                CreationDate = DateTime.UtcNow,
                ModifiedBy = "System"
            };
            await _repository.AddAsync(country);
            return country.ToDto();
        }

        public async Task<CountryDto> UpdateAsync(UpdateCountryDto dto)
        {
            var country = await _repository.GetByIdAsync(dto.Id) ?? throw new NotFoundException("Country not found");
            country.UpdateFromDto(dto);

            country.ModificationDate = DateTime.UtcNow;
            country.ModifiedBy = "System";

            await _repository.UpdateAsync(country);
            return country.ToDto();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid country ID.");
            var country = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Country not found");
            await _repository.DeleteAsync(country.Id);
            return true;
        }
    }
}
