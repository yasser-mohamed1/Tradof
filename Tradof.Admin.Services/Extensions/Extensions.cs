using Microsoft.AspNetCore.Identity;
using Tradof.Admin.Services.DataTransferObject.AuthenticationDto;
using Tradof.Data.Entities;

namespace Tradof.Admin.Services.Extensions
{
    public static class RegisterAdminDtoExtensions
    {
        public static ApplicationUser ToApplicationUser(this RegisterAdminDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
        }

        public static GetUserDto ToGetUserDto(this ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return new GetUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };

        }
    }
}
