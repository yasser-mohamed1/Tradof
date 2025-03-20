using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tradof.Auth.Services.DTOs;
using Tradof.Auth.Services.Interfaces;

namespace Tradof.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {

        [Authorize]
        [HttpGet("token")]
        public async Task<IActionResult> GetToken()
        {
            var user = HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "User is not authenticated" });
            }

            var token = await HttpContext.GetTokenAsync("Bearer", "access_token");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Token not found in context" });
            }

            return Ok(new { Token = token });
        }


        [HttpPost("register-company")]
        public async Task<IActionResult> RegisterCompany([FromBody] RegisterCompanyDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _authService.RegisterCompanyAsync(dto);
                return Ok("Registration successful. Please check your email to confirm your account.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("register-freelancer")]
        public async Task<IActionResult> RegisterFreelancer([FromBody] RegisterFreelancerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _authService.RegisterFreelancerAsync(dto);
                return Ok("Registration successful. Please check your email to confirm your account.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("log-in")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var (token, refreshToken, userId, role) = await _authService.LoginAsync(dto);

                return Ok(new { UserId = userId, Role = role, Token = token, RefreshToken = refreshToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var (accessToken, refreshToken) = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (accessToken is null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
        {
            var result = await _authService.ConfirmEmailAsync(email, token);
            if (result)
            {
                return Ok("Email confirmed successfully.");
            }
            return BadRequest("Invalid email or token.");
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _authService.ForgetPasswordAsync(dto);
                return Ok("Password reset instructions have been sent to your email.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _authService.VerifyOtpAsync(dto);
                var resetToken = await _authService.GeneratePasswordResetTokenAsync(dto.Email);
                return Ok(new { ResetToken = resetToken });
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _authService.ChangePasswordWithTokenAsync(dto);
                return Ok("Password changed successfully.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var (id, role) = await _authService.GetCurrentUserAsync();
            return Ok(new
            {
                Id = id,
                Role = role
            });
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _authService.ResendOtpAsync(dto.Email);
                return Ok("OTP has been resent to your email.");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}