using System;
using System.Text;
using Baz.MassTransit.Extensions.RabbitMq.Extensions;
using Baz.Service.Action.AspNetCore.Extensions;
using Baz.Service.Action.Core;
using Expense.Collector.Synchronization;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Payment.Tracker.Api.Extensions;
using Payment.Tracker.BusinessLogic.Configuration;
using Payment.Tracker.BusinessLogic.EventHandlers;
using Payment.Tracker.BusinessLogic.Seeds;
using Payment.Tracker.BusinessLogic.Services;
using Payment.Tracker.BusinessLogic.Statistics;
using Payment.Tracker.BusinessLogic.Statistics.Providers;
using Payment.Tracker.BusinessLogic.Validators.Template;
using Payment.Tracker.DataLayer;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors()
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddFluentValidation(
                    configuration =>
                    {
                        configuration.RegisterValidatorsFromAssemblyContaining<PaymentPositionTemplateDtoValidator>();
                        configuration.ImplicitlyValidateChildProperties = true;
                        configuration.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                        configuration.LocalizationEnabled = true;
                    });
            
            var securityConfigSection = Configuration.GetSection(nameof(SecuritySettings));
            var securitySettings = securityConfigSection.Get<SecuritySettings>();
            ApplyEnvironmentVariables(securitySettings);

            services.AddSingleton<ISecuritySettings>(securitySettings);
            ConfigureAuth(services, securitySettings);

            var builder = new ServiceActionResponseMapBuilder();
            services.AddControllers(options => { options
                .Filters
                .AddServiceActionFilter(builder.Build());});

            var connectionString = Configuration.GetConnectionString(Consts.DatabaseName);
            var connectionStringVar = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(connectionStringVar))
            {
                connectionString = connectionStringVar;
            }

            var context = new PaymentContext(connectionString);
            services.AddSingleton(context);

            ConfigureMassTransit(services);
            
            RegisterRepositories(services);
            RegisterServices(services);
            RegisterStatisticDataProviders(services);
            RegisterSeeds(services);
        }

        private void ConfigureMassTransit(IServiceCollection services)
        {
            services.AddMassTransitWithRabbitMq(Configuration, configurator =>
            {
                configurator.WithConsumer<SharedExpenseUpdatedEventHandler>(Queues.ExpenseEvents);
                configurator.WithConsumer<SharedExpenseDeletedEventHandler>(Queues.ExpenseEvents);
            });

            services.AddMassTransitHostedService();
        }
        
        private static void ApplyEnvironmentVariables(ISecuritySettings securitySettings)
        {
            var tokenSecretVar = Environment.GetEnvironmentVariable("TOKEN_SECRET");
            if (!string.IsNullOrWhiteSpace(tokenSecretVar))
            {
                securitySettings.TokenSecret = tokenSecretVar;
            }

            var appHost = Environment.GetEnvironmentVariable("APP_HOST");
            if (!string.IsNullOrWhiteSpace(appHost))
            {
                securitySettings.AllowedHost = appHost;
            }

            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
            if (!string.IsNullOrWhiteSpace(adminPassword))
            {
                securitySettings.AdminPassword = adminPassword;
            }
        }
        
        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<IGenericRepository<User>, GenericRepository<User>>();
            services.AddSingleton<IGenericRepository<PaymentPositionTemplate>, GenericRepository<PaymentPositionTemplate>>();
            services.AddSingleton<IGenericRepository<PaymentSet>, GenericRepository<PaymentSet>>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IPaymentsService, PaymentsService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
        }

        private static void RegisterStatisticDataProviders(IServiceCollection services)
        {
            services.AddScoped<IStatisticsTypeStrategy, StatisticsTypeStrategy>();
            services.AddScoped<IStatisticsDataProvider, TotalCostsPerMonthDataProvider>();
        }
        
        private static void RegisterSeeds(IServiceCollection services)
        {
            services.AddSingleton<ISeed, UserSeed>();
        }
        
        private static void ConfigureAuth(IServiceCollection services, ISecuritySettings securitySettings)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var secretBytes = Encoding.ASCII.GetBytes(securitySettings.TokenSecret);
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            var securitySettings = app.ApplicationServices.GetService<ISecuritySettings>();

            app.UseCors(options =>
                options.WithOrigins(securitySettings?.AllowedHost)
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            app.ConfigureExceptionHandler();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            SeedDatabase(app);
        }
        
        private static void SeedDatabase(IApplicationBuilder app)
        {
            var seeders = app.ApplicationServices.GetServices<ISeed>();
            foreach (var seeder in seeders)
            {
                seeder.SeedAsync().GetAwaiter().GetResult();
            }
        }
    }
}