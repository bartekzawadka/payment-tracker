using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Payment.Tracker.Notifier.Jobs;
using Payment.Tracker.Notifier.Models;
using Quartz;

namespace Payment.Tracker.Notifier.Extensions
{
    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        public static void AddJobsAndTriggers(
            this IServiceCollectionQuartzConfigurator quartz,
            IConfiguration config)
        {
            IConfigurationSection quartzConfigSection = config.GetSection(nameof(QuartzConfiguration));
            var quartzConfiguration = quartzConfigSection.Get<QuartzConfiguration>();
            if (quartzConfiguration?.Triggers == null || quartzConfiguration.Triggers.Count == 0)
            {
                throw new ApplicationException("Wrong QUARTZ configuration. Missing triggers");
            }

            foreach (var notificationJobTriggerConfiguration in quartzConfiguration.Triggers)
            {
                var jobKey = new JobKey(notificationJobTriggerConfiguration.Name);
                var jobData = new Dictionary<string, object>
                {
                    {"NotificationType", (int)notificationJobTriggerConfiguration.Type}
                };
                quartz.AddJob<EmailNotifierJob>(opts => opts
                    .WithIdentity(jobKey)
                    .SetJobData(new JobDataMap((IDictionary<string, object>)jobData)));
                quartz.AddTrigger(configurator => configurator
                    .ForJob(jobKey)
                    .WithIdentity($"{notificationJobTriggerConfiguration.Name}-trigger")
                    .WithCronSchedule(notificationJobTriggerConfiguration.Cron));
            }
        }
    }
}