using System.Collections.Generic;

namespace Payment.Tracker.Notifier.Models
{
    public class QuartzConfiguration
    {
        public List<NotificationJobTriggerConfiguration> Triggers { get; set; }
    }
}