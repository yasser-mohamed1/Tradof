namespace Tradof.Project.Services.DTOs
{
    public class NotificationDto
    {
        public string type { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string message { get; set; }
        public string description { get; set; }
        public bool seen { get; set; }
        public DateTime timestamp { get; set; }
    }

    public enum NotificationType
    {
        TechnicalSupport,
        Offer,
        Payment,
        Message,
        Calendar,
        Project,
        Feedback,
        Evaluation,
        Report,
        Subscriptions,
        WithdrawProfit,
        AskQuestion
    }
}