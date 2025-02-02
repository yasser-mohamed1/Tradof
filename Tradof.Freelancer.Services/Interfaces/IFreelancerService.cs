using Microsoft.AspNetCore.Http;
using Tradof.Data.Entities;
using Tradof.FreelancerModule.Services.DTOs;

namespace Tradof.FreelancerModule.Services.Interfaces
{
    public interface IFreelancerService
    {
        Task<FreelancerDTO> GetFreelancerDataAsync(string userId);
        Task<string> UploadCVAsync(string Id, IFormFile file);
        Task<bool> UpdateFreelancerAsync(string Id, UpdateFreelancerDTO updatedData);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO dto);
        Task<bool> AddPaymentMethodAsync(string Id, PaymentMethodDTO dto);
        Task<bool> RemovePaymentMethodAsync(long Id);
        Task<bool> AddFreelancerSocialMediaAsync(string Id, AddFreelancerSocialMediaDTO dto);
        Task<bool> UpdateFreelancerSocialMediaAsync(long socialMediaId, UpdateFreelancerSocialMediaDTO dto);
        Task<bool> AddFreelancerLanguagePairAsync(string Id, AddFreelancerLanguagePairDTO dto);
        Task<bool> RemoveFreelancerLanguagePairAsync(long Id);
    }
}