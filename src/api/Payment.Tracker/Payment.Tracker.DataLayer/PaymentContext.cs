using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.DataLayer
{
    public class PaymentContext : DbContext
    {
        public DbSet<PaymentPosition> PaymentPositions { get; set; }

        public DbSet<PaymentSet> PaymentSets { get; set; }
        
        public DbSet<PaymentPositionTemplate> PaymentPositionTemplates { get; set; }
        
        public DbSet<User> Users { get; set; }

        public PaymentContext()
        {
        }

        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }
            
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            string appSettingsDir = Path.Combine(Directory.GetCurrentDirectory(), "..", assemblyName.Name ?? string.Empty);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(appSettingsDir)
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString(Consts.DatabaseName);
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(Consts.DbServerVersion));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentSet>(builder =>
            {
                builder
                    .HasMany(e => e.PaymentPositions)
                    .WithOne(position => position.PaymentSet)
                    .HasForeignKey(x => x.PaymentSetId);
            });

            modelBuilder.Entity<PaymentPosition>(builder =>
            {
                builder.Property(position => position.Name).IsRequired();
            });
            
            modelBuilder.Entity<User>(builder =>
            {
                builder.Property(user => user.UserName).IsRequired();
                builder.Property(user => user.PasswordHash).IsRequired();
                builder.Property(user => user.PasswordSalt).IsRequired();
            });

            modelBuilder.Entity<PaymentPositionTemplate>(builder =>
                builder.Property(template => template.Name).IsRequired());
        }
    }
}