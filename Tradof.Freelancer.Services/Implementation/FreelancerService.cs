using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tradof.Common.Enums;
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
                .Include(f => f.Specializations)
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

        public async Task AddOrUpdateOrRemoveFreelancerSocialMediasAsync(string id, IEnumerable<AddFreelancerSocialMediaDTO> socialMedias)
        {
            var freelancer = await _context.Freelancers
                .Include(f => f.FreelancerSocialMedias)
                .FirstOrDefaultAsync(f => f.UserId == id);

            if (freelancer == null)
                throw new ArgumentException("Invalid Freelancer ID.");

            var mediaPlatformTypes = socialMedias.Select(sm => sm.PlatformType.ToLower()).ToList();

            // Remove medias with empty links
            var mediasToRemove = freelancer.FreelancerSocialMedias
                .Where(m => socialMedias.Any(dto => dto.PlatformType.Equals(m.PlatformType.ToString(), StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(dto.Link)))
                .ToList();

            if (mediasToRemove.Any())
            {
                _context.RemoveRange(mediasToRemove);
            }

            // Update existing or add new medias
            foreach (var dto in socialMedias)
            {
                if (!Enum.TryParse<PlatformType>(dto.PlatformType, true, out var platformType))
                    throw new ArgumentException($"Invalid PlatformType: {dto.PlatformType}");

                if (string.IsNullOrWhiteSpace(dto.Link)) continue;

                var existingMedia = freelancer.FreelancerSocialMedias.FirstOrDefault(m => m.PlatformType == platformType);

                if (existingMedia != null)
                {
                    existingMedia.Link = dto.Link;
                }
                else
                {
                    var socialMediaEntity = dto.ToFreelancerSocialMediaEntity(freelancer.Id);
                    freelancer.FreelancerSocialMedias.Add(socialMediaEntity);
                }
            }

            await _context.SaveChangesAsync();
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

        public async Task<bool> AddFreelancerLanguagePairsAsync(string id, IEnumerable<AddFreelancerLanguagePairDTO> dtos)
        {
            var freelancer = await _context.Freelancers.FirstOrDefaultAsync(f => f.UserId == id);
            if (freelancer == null) return false;

            var newLanguagePairs = new List<FreelancerLanguagesPair>();

            foreach (var dto in dtos)
            {
                var existingPair = await _context.FreelancerLanguagesPairs
                    .FirstOrDefaultAsync(lp => lp.FreelancerId == freelancer.Id &&
                                                lp.LanguageFromId == dto.LanguageFromId &&
                                                lp.LanguageToId == dto.LanguageToId);
                if (existingPair == null)
                {
                    var languagePair = dto.ToFreelancerLanguagePair(freelancer.Id);
                    newLanguagePairs.Add(languagePair);
                }
            }

            if (!newLanguagePairs.Any()) return false;

            await _context.FreelancerLanguagesPairs.AddRangeAsync(newLanguagePairs);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFreelancerLanguagePairsAsync(string id, IEnumerable<long> ids)
        {
            var freelancer = await _context.Freelancers.FirstOrDefaultAsync(f => f.UserId == id);
            if (freelancer == null) return false;

            var languagePairs = await _context.FreelancerLanguagesPairs
                .Where(lp => lp.FreelancerId == freelancer.Id && ids.Contains(lp.Id))
                .ToListAsync();

            if (!languagePairs.Any()) return false;

            _context.FreelancerLanguagesPairs.RemoveRange(languagePairs);
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