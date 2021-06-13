using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payment.Tracker.DataLayer;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;
using Payment.Tracker.Notifier.Email;
using Payment.Tracker.Notifier.Email.NotificationProviders;
using Payment.Tracker.Notifier.Extensions;
using Payment.Tracker.Notifier.Models;
using Payment.Tracker.Utils.EnvironmentVariables;
using Quartz;

namespace Payment.Tracker.Notifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString(Consts.DatabaseName);
            var connectionStringVar = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(connectionStringVar))
            {
                connectionString = connectionStringVar;
            }
            
            var context = new PaymentContext(connectionString);
            services.AddSingleton(context);

            services.AddQuartz(configurator =>
            {
                configurator.UseMicrosoftDependencyInjectionScopedJobFactory();
                configurator.AddJobsAndTriggers(Configuration);
            });
            
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            IConfigurationSection emailConfigSection = Configuration.GetSection(nameof(EmailConfiguration));
            var emailConfiguration = emailConfigSection.Get<EmailConfiguration>();
            UpdateConfigBasedOnEnvVars(emailConfiguration);
            services.AddSingleton(emailConfiguration);
            
            RegisterRepositories(services);
            RegisterServices(services);
        }

        private static void UpdateConfigBasedOnEnvVars(EmailConfiguration emailConfiguration)
        {
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, string>(emailConfiguration, configuration => configuration.Password, "NOTIFIER_EMAIL_PASSWORD");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, int>(emailConfiguration, configuration => configuration.Port, "NOTIFIER_EMAIL_PORT");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, string>(emailConfiguration, configuration => configuration.Server, "NOTIFIER_EMAIL_SERVER");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, string>(emailConfiguration, configuration => configuration.To, "NOTIFIER_EMAIL_TO");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, string>(emailConfiguration, configuration => configuration.Username, "NOTIFIER_EMAIL_USERNAME");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, string>(emailConfiguration, configuration => configuration.AppEndpoint, "NOTIFIER_EMAIL_APP_ENDPOINT");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, string>(emailConfiguration, configuration => configuration.FromAddress, "NOTIFIER_EMAIL_FROM_ADDRESS");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, string>(emailConfiguration, configuration => configuration.FromName, "NOTIFIER_EMAIL_FROM_NAME");
            EnvironmentVariablesHelper.SetValueFromEnvVar<EmailConfiguration, bool>(emailConfiguration, configuration => configuration.UseSsl, "NOTIFIER_EMAIL_USE_SSL");
        }
        
        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<IGenericRepository<PaymentSet>, GenericRepository<PaymentSet>>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IEmailNotificationStrategy, EmailNotificationStrategy>();

            services.AddScoped<IEmailNotificationHandler, PrecursoryNotificationHandler>();
            services.AddScoped<IEmailNotificationHandler, CreateSetReminderNotificationHandler>();
            services.AddScoped<IEmailNotificationHandler, ClosePeriodReminderNotificationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}