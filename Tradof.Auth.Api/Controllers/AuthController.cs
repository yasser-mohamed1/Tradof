using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Tradof.Auth.Services.DTOs;
using Tradof.Auth.Services.Interfaces;

namespace Tradof.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("user-data-with-token")]
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
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var firstName = user.FindFirst("firstName")?.Value;
            var lastName = user.FindFirst("lastName")?.Value;
            var profileImageUrl = user.FindFirst("profileImageUrl")?.Value;
            var userType = user.FindFirst("userType")?.Value;

            return Ok(new
            {
                id,
                email,
                role,
                firstName,
                lastName,
                profileImageUrl,
                userType
            });
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

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _authService.GetUserById(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse));
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var (token, refreshToken, userId, role) = await _authService.AuthenticateWithGoogle(HttpContext);

                var html = $@"
        <html>
            <body>
                <script>
                    window.opener?.postMessage({{
                        token: '{token}',
                        refreshToken: '{refreshToken}',
                        userId: '{userId}',
                        role: '{role}'
                    }}, '*');
                    window.close();
                </script>
                <p>Authentication successful. You can close this window.</p>
            </body>
        </html>";

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                var html = $@"
        <html>
            <body>
                <script>
                    window.opener?.postMessage({{
                        error: '{ex.Message}'
                    }}, '*');
                    window.close();
                </script>
                <p>Authentication failed. You can close this window.</p>
            </body>
        </html>";

                return Content(html, "text/html");
            }
        }
    }
}