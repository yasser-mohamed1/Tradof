using System.ComponentModel.DataAnnotations;
using Tradof.Common.Exceptions;
using Tradof.Data.Entities;
using Tradof.Data.Interfaces;
using Tradof.SpecializationModule.Services.DTOs;
using Tradof.SpecializationModule.Services.Extensions;
using Tradof.SpecializationModule.Services.Interfaces;

namespace Tradof.SpecializationModule.Services.Implementation
{
    public class SpecializationService(IGeneralRepository<Specialization> _repository) : ISpecializationService
    {
        public async Task<IEnumerable<SpecializationDto>> GetAllAsync()
        {
            var specializations = await _repository.GetAllAsync();
            return specializations.Select(s => s.ToDto());
        }

        public async Task<SpecializationDto> GetByIdAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid specialization ID.");
            var specialization = await _repository.GetByIdAsync(id);
            return specialization == null ? throw new NotFoundException("Specialization not found") : specialization.ToDto();
        }

        public async Task<SpecializationDto> CreateAsync(CreateSpecializationDto dto)
        {

            var specialization = new Specialization
            {
                Name = dto.Name,
                CreatedBy = "System",
                CreationDate = DateTime.UtcNow,
                ModifiedBy = "System"
            };
            await _repository.AddAsync(specialization);
            return specialization.ToDto();
        }

        public async Task<SpecializationDto> UpdateAsync(UpdateSpecializationDto dto)
        {
            var specialization = await _repository.GetByIdAsync(dto.Id) ?? throw new NotFoundException("Specialization not found");
            specialization.UpdateFromDto(dto);

            specialization.ModificationDate = DateTime.UtcNow;
            specialization.ModifiedBy = "System";

            await _repository.UpdateAsync(specialization);
            return specialization.ToDto();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0) throw new ValidationException("Invalid specialization ID.");
            var specialization = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Specialization not found");
            await _repository.DeleteAsync(specialization.Id);
            return true;
        }
    }
}
