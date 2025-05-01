using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using Tradof.Common.Enums;
using Tradof.Data.Interfaces;
using Tradof.EntityFramework.Helpers;
using Tradof.Project.Services.Interfaces;

namespace Tradof.Project.Services.Implementation
{
    public class ProjectNotificationService : IProjectNotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _notificationApiUrl;
        private readonly string _systemUserId;

        public ProjectNotificationService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _notificationApiUrl = _configuration["NotificationApi:Url"] ?? "https://tradofserver.azurewebsites.net/api/notification/send";
            _systemUserId = _configuration["System:UserId"] ?? "22166f09-8e78-419e-a3ed-6b61f8983406";
        }

        public async Task SendProjectEndDateNotificationsAsync()
        {
            var tomorrow = DateTime.UtcNow.AddDays(1).Date;

            var endingProjects = await _unitOfWork.Repository<Data.Entities.Project>()
                .FindAsync(p =>
                    p.Status == ProjectStatus.Active &&
                    p.EndDate.Date == tomorrow &&
                    p.Freelancer != null);

            var httpClient = _httpClientFactory.CreateClient();

            foreach (var project in endingProjects)
            {
                if (project.Freelancer?.User?.Email == null) continue;

                var emailBody = $@"
                    <h2>Project Deadline Reminder</h2>
                    <p>Dear {project.Freelancer.User.FirstName},</p>
                    <p>This is a reminder that your project '{project.Name}' is ending tomorrow ({project.EndDate:MM/dd/yyyy}).</p>
                    <p>Please make sure to complete all necessary tasks and deliverables before the deadline.</p>
                    <p>Project Details:</p>
                    <ul>
                        <li>Project Name: {project.Name}</li>
                        <li>End Date: {project.EndDate:MM/dd/yyyy}</li>
                        <li>Description: {project.Description}</li>
                    </ul>
                    <p>Best regards,<br>Tradof Team</p>";

                await _emailService.SendEmailAsync(
                    project.Freelancer.User.Email,
                    $"Project Deadline Reminder: {project.Name}",
                    emailBody
                );

                var notificationRequest = new
                {
                    type = "Finish_or_Deadline",
                    senderId = _systemUserId,
                    message = $"Your project '{project.Name}' is ending tomorrow.",
                    description = $"Project '{project.Name}' deadline is approaching. Please ensure all deliverables are completed by {project.EndDate:MM/dd/yyyy}."
                };

                var jsonContent = JsonSerializer.Serialize(notificationRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(_notificationApiUrl, content);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send notification for project {project.Id}: {ex.Message}");
                }
            }
        }
    }
}