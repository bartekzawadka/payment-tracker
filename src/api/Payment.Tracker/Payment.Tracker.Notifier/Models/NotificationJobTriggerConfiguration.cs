namespace Payment.Tracker.Notifier.Models
{
    public class NotificationJobTriggerConfiguration
    {
        public string Name { get; set; }
        
        public string Cron { get; set; }

        public NotificationType Type { get; set; }
    }
}