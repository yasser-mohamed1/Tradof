using System.ComponentModel.DataAnnotations;
using Tradof.Auth.Services.DTOs;

namespace Tradof.Auth.Services.Validation
{
    public static class ValidationHelper
    {
        public static void ValidateRegisterCompanyDto(RegisterCompanyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ValidationException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ValidationException("Last name cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.CompanyAddress))
                throw new ValidationException("Company address cannot be empty.");
            if (dto.CountryId <= 0)
                throw new ValidationException("Invalid Country ID.");
            if (dto.SpecializationId <= 0)
                throw new ValidationException("Invalid Specialization ID.");
        }

        public static void ValidateRegisterFreelancerDto(RegisterFreelancerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ValidationException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ValidationException("Last name cannot be empty.");
            if (dto.WorkExperience < 0)
                throw new ValidationException("Work experience cannot be negative.");
            if (dto.SpecializationId <= 0)
                throw new ValidationException("Invalid Specialization ID.");
        }

        public static void ValidateLoginDto(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ValidationException("Password cannot be empty.");
        }

        public static void ValidateForgetPasswordDto(ForgetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email cannot be empty.");
        }

        public static void ValidateOtpVerificationDto(OtpVerificationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(dto.Otp))
                throw new ValidationException("OTP cannot be empty.");
        }
    }
}