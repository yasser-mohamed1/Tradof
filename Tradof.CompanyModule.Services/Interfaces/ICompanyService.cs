using Tradof.CompanyModule.Services.DTOs;

namespace Tradof.CompanyModule.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyDto?> GetByIdAsync(string id);
        Task AddEmployeeAsync(CreateCompanyEmployeeDto dto);
        Task<CompanySubscriptionDto?> GetCurrentSubscriptionAsync(string userId);
        Task<IEnumerable<EmployeeDto>> GetCompanyEmployeesAsync(string companyId);
        Task UpdateCompanyAsync(UpdateCompanyDto dto);
        Task AddLanguagesAsync(string companyId, IEnumerable<long> languageIds);
        Task RemoveLanguagesAsync(string companyId, IEnumerable<long> languageIds);
        Task AddSpecializationsAsync(string companyId, IEnumerable<long> specializationIds);
        Task RemoveSpecializationsAsync(string companyId, IEnumerable<long> specializationIds);
        Task<bool> ChangeCompanyPasswordAsync(string Id, ChangeCompanyPasswordDto dto);
        Task AddOrUpdateOrRemoveSocialMediasAsync(string id, IEnumerable<CreateSocialMediaDto> socialMedias);
        Task<CompanyDto?> GetByEmployeeIdAsync(string employeeId);
    }
}
