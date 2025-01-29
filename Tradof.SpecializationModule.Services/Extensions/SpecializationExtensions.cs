using Tradof.Data.Entities;
using Tradof.SpecializationModule.Services.DTOs;

namespace Tradof.SpecializationModule.Services.Extensions
{
    public static class SpecializationExtensions
    {
        public static SpecializationDto ToDto(this Specialization specialization)
        {
            return new SpecializationDto(
                specialization.Id,
                specialization.Name
                //specialization.CreationDate,
                //specialization.ModificationDate,
                //specialization.CreatedBy,
                //specialization.ModifiedBy
            );
        }

        public static void UpdateFromDto(this Specialization specialization, UpdateSpecializationDto dto)
        {
            specialization.Name = dto.Name;

            if (dto.ModificationDate.HasValue)
            {
                specialization.ModificationDate = dto.ModificationDate;
            }
            if (!string.IsNullOrEmpty(dto.ModifiedBy))
            {
                specialization.ModifiedBy = dto.ModifiedBy;
            }
        }
    }
}
