using System.Threading.Tasks;
using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Interfaces
{
    public interface INotificationService
    {
        Task<(bool success, string message)> SendNotificationAsync(NotificationDto notification);
    }
}