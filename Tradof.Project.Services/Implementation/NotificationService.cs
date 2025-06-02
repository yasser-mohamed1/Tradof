using System.Text;
using System.Text.Json;
using Tradof.Project.Services.DTOs;
using Tradof.Project.Services.Interfaces;

namespace Tradof.Project.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://tradofapi-production.up.railway.app/api/notification";

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool success, string message)> SendNotificationAsync(NotificationDto notification)
        {
            try
            {
                var json = JsonSerializer.Serialize(notification);
                Console.WriteLine($"Sending notification with payload: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"{BaseUrl}/send";
                Console.WriteLine($"Sending request to: {url}");
                Console.WriteLine(content);
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Notification sent successfully");
                }

                return (false, $"Failed to send notification: {responseContent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return (false, $"Error sending notification: {ex.Message}");
            }
        }
    }
}