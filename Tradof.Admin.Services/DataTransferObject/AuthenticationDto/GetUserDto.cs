﻿using System.ComponentModel.DataAnnotations;

namespace Tradof.Admin.Services.DataTransferObject.AuthenticationDto
{
    public class GetUserDto
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Name is required"),
        MaxLength(1000, ErrorMessage = "Name cannot exceed 500 characters"),
        MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Name is required"),
        MaxLength(1000, ErrorMessage = "Name cannot exceed 500 characters"),
        MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required"), MaxLength(11, ErrorMessage = "Phone number cannot exceed 11 digits")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required"), DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string Role { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool? EmailConfirmed { get; set; }
    }
}
