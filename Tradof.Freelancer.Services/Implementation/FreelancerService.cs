using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tradof.Data.Entities;
using Tradof.EntityFramework.DataBase_Context;
using Tradof.FreelancerModule.Services.DTOs;
using Tradof.FreelancerModule.Services.Extensions;
using Tradof.FreelancerModule.Services.Interfaces;

namespace Tradof.FreelancerModule.Services.Implementation
{
    public class FreelancerService(
        UserManager<ApplicationUser> userManager,
        TradofDbContext _context) : IFreelancerService
    {
        Cloudinary _cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));

        public async Task<FreelancerDTO> GetFreelancerDataAsync(string userId)
        {
            var freelancer = await _context.Freelancers
                .Include(f => f.User)
                .Include(f => f.Country)
                .Include(f => f.Projects)
                .ThenInclude(p => p.Ratings)
                .Include(f => f.FreelancerLanguagesPairs)
                .ThenInclude(lp => lp.LanguageFrom)
                .Include(f => f.FreelancerLanguagesPairs)
                .ThenInclude(lp => lp.LanguageTo)
                .Include(f => f.FreelancerSocialMedias)
                .FirstOrDefaultAsync(f => f.UserId == userId);

            return freelancer is null ? throw new ArgumentNullException("No Freelancer with this Id.") : freelancer.ToFreelancerDTO();
        }

        public async Task<string> UploadCVAsync(string Id, IFormFile file)
        {
            var freelancer = await _context.Freelancers.FirstOrDefaultAsync(f => f.UserId == Id);
            if (freelancer == null) return null;

            if (!string.IsNullOrEmpty(freelancer.CVFilePath))
            {
                var deleteParams = new DeletionParams(freelancer.CVFilePath);
                _cloudinary.Destroy(deleteParams);
            }

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            freelancer.CVFilePath = uploadResult.SecureUrl.ToString();

            await _context.SaveChangesAsync();
            return freelancer.CVFilePath;
        }

        public async Task<bool> UpdateFreelancerAsync(string Id, UpdateFreelancerDTO dto)
        {
            var freelancer = await _context.Freelancers
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.UserId == Id);

            if (freelancer == null) return false;

            freelancer.UpdateFromDto(dto);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddFreelancerSocialMediaAsync(string Id, AddFreelancerSocialMediaDTO dto)
        {
            var freelancer = await _context.Freelancers
                .FirstOrDefaultAsync(f => f.UserId == Id);
            if (freelancer == null)
                return false;

            var socialMediaEntity = dto.ToFreelancerSocialMediaEntity(freelancer.Id);
            _context.FreelancerSocialMedias.Add(socialMediaEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateFreelancerSocialMediaAsync(long socialMediaId, UpdateFreelancerSocialMediaDTO dto)
        {
            var socialMedia = await _context.FreelancerSocialMedias.FirstOrDefaultAsync(sm => sm.Id == socialMediaId);

            if (socialMedia == null) return false;

            socialMedia.Link = dto.Link;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword) return false;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> AddPaymentMethodAsync(string Id, PaymentMethodDTO dto)
        {
            var freelancer = await _context.Freelancers.FirstOrDefaultAsync(f => f.UserId == Id);
            if (freelancer == null) return false;

            if (!ValidatePaymentMethod(dto)) return false;

            freelancer.PaymentMethods.Add(dto.ToPaymentMethodEntity());

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePaymentMethodAsync(long Id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(Id);
            if (paymentMethod == null) return false;

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddFreelancerLanguagePairAsync(string Id, AddFreelancerLanguagePairDTO dto)
        {
            var freelancer = await _context.Freelancers.FirstOrDefaultAsync(f => f.UserId == Id);
            if (freelancer == null) return false;

            var existingPair = await _context.FreelancerLanguagesPairs
                .FirstOrDefaultAsync(lp => lp.FreelancerId == freelancer.Id &&
                                            lp.LanguageFromId == dto.LanguageFromId &&
                                            lp.LanguageToId == dto.LanguageToId);

            if (existingPair != null) return false;

            var languagePair = dto.ToFreelancerLanguagePair(freelancer.Id);

            await _context.FreelancerLanguagesPairs.AddAsync(languagePair);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFreelancerLanguagePairAsync(long Id)
        {
            var languagePair = await _context.FreelancerLanguagesPairs.FindAsync(Id);
            if (languagePair == null) return false;

            _context.FreelancerLanguagesPairs.Remove(languagePair);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool ValidatePaymentMethod(PaymentMethodDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CardNumber) || dto.CardNumber.Length < 13)
                return false;

            if (!DateTime.TryParseExact($"01/{dto.ExpiryDate}", "dd/MM/yy", null, System.Globalization.DateTimeStyles.None, out var expiry))
                return false;

            if (expiry <= DateTime.Now)
                return false;

            if (dto.CVV < 100 || dto.CVV > 9999)
                return false;

            return true;
        }
    }
}