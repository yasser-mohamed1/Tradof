using System.Threading.Tasks;

namespace Tradof.Project.Services.Interfaces
{
    public interface IProjectNotificationService
    {
        Task SendProjectEndDateNotificationsAsync();
    }
}