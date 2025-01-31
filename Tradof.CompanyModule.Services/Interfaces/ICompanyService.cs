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
        Task AddLanguageAsync(string companyId, long languageId);
        Task RemoveLanguageAsync(string companyId, long languageId);
        Task AddSpecializationAsync(string companyId, long specializationId);
        Task RemoveSpecializationAsync(string companyId, long specializationId);
        Task ChangeCompanyPasswordAsync(ChangeCompanyPasswordDto dto);
    }
}
