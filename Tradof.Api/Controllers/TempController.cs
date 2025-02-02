using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;

namespace Tradof.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempController(TradofDbContext _context) : ControllerBase
    {
        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return NotFound("Account not found.");
                }

                var freelancer = await _context.Freelancers.FirstOrDefaultAsync(f => f.UserId == user.Id);

                var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserId == user.Id);
                // Check if the user is a Freelancer and delete all associated data
                if (freelancer != null)
                {
                    // Delete Freelancer related data
                    var paymentMethods = await _context.PaymentMethods.Where(pm => pm.FreelancerId == freelancer.Id).ToListAsync();
                    _context.PaymentMethods.RemoveRange(paymentMethods);
                    _context.Proposals.RemoveRange(freelancer.Proposals);
                    _context.FreelancerSocialMedias.RemoveRange(freelancer.FreelancerSocialMedias);
                    _context.FreelancerLanguagesPairs.RemoveRange(freelancer.FreelancerLanguagesPairs);
                    _context.Specializations.RemoveRange(freelancer.Specializations);
                    _context.Projects.RemoveRange(freelancer.Projects);

                    _context.Freelancers.Remove(freelancer);
                }
                // Check if the user is a Company and delete all associated data
                else if (company != null)
                {
                    // Delete Company related data
                    _context.CompanySubscriptions.RemoveRange(company.Subscriptions);
                    _context.Projects.RemoveRange(company.Projects);
                    _context.Specializations.RemoveRange(company.Specializations);
                    _context.CompanySocialMedias.RemoveRange(company.Medias);
                    _context.CompanyEmployees.RemoveRange(company.Employees);
                    _context.Languages.RemoveRange(company.PreferredLanguages);

                    _context.Companies.Remove(company);
                }
                else
                {
                    // Handle other user types or default case (if needed)
                    _context.Users.Remove(user);  // Removing other types of users directly
                }

                // Remove the user
                _context.Users.Remove(user);

                // Commit the transaction to delete all data
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok($"Account with email {email} and all associated data has been deleted.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
