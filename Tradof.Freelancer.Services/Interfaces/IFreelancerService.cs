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
        Task<bool> AddFreelancerLanguagePairsAsync(string id, IEnumerable<AddFreelancerLanguagePairDTO> dtos);
        Task<bool> RemoveFreelancerLanguagePairsAsync(string id, IEnumerable<long> ids);
        Task AddOrUpdateOrRemoveFreelancerSocialMediasAsync(string id, IEnumerable<AddFreelancerSocialMediaDTO> socialMedias);
        Task AddSpecializationsAsync(string Id, IEnumerable<long> specializationIds);
        Task RemoveSpecializationsAsync(string Id, IEnumerable<long> specializationIds);
        Task<SetExamScoreResponse> SetExamScoreAsync(SetExamScoreRequest request);
    }
}