namespace Tradof.Auth.Services.Interfaces
{
    public interface IRoleRepository
    {
        Task<string> GetUserRoleAsync(string userId);
    }
}
