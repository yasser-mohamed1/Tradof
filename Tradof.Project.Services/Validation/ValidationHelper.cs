using System.ComponentModel.DataAnnotations;
using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Validation
{
    public static class ValidationHelper
    {
        public static void ValidateCreateProjectDto(CreateProjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("title cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ValidationException("Description cannot be empty.");
            if (dto.MinPrice <= 0)
                throw new ValidationException("Price must be greater than 0.");
            if (dto.Days <= 0)
                throw new ValidationException("Duration must be greater than 0.");
        }

        public static void ValidateUpdateProjectDto(UpdateProjectDto dto)
        {
            if (dto.Id <= 0)
                throw new ValidationException("Invalid project ID.");
            ValidateCreateProjectDto(new CreateProjectDto { Name = dto.Name, Description = dto.Description, MinPrice = dto.MinPrice, Days = dto.Days });
        }
    }
}