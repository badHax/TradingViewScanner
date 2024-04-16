using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using TVScanner.Shared.Scanner;

namespace TVScanner.Data
{
    public class TVScannerContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        DbSet<ScanRecord> ScanRecords { get; set; }
        public IConfiguration Configuration { get; }

        public TVScannerContext(DbContextOptions<TVScannerContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions,
        IConfiguration configuration) : base(options, operationalStoreOptions)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.LogTo(
                Console.WriteLine,
                new[] {
                    RelationalEventId.CommandError
                },
                LogLevel.Information,
                DbContextLoggerOptions.DefaultWithLocalTime | DbContextLoggerOptions.SingleLine);


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScanRecord>()
                .HasKey(r => r.Name);

            modelBuilder.Entity<ScanRecord>().ToTable("ScanRecord");

            base.OnModelCreating(modelBuilder);
        }

        public void Seed()
        {
            Database.Migrate();

            //ROLES
            if (!Roles.Any())
            {
                Roles.Add(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" });
                Roles.Add(new IdentityRole { Name = "User", NormalizedName = "USER" });
                SaveChanges();
            }

            //USERS
            if (!Users.Any())
            {
                var hasher = new PasswordHasher<ApplicationUser>();
                var userName = Configuration.GetValue<string>("Users:DefaultAdmin:Name");
                var email = Configuration.GetValue<string>("Users:DefaultAdmin:Email");
                var password = Configuration.GetValue<string>("Users:DefaultAdmin:Password");
                var user = new ApplicationUser
                {
                    UserName = userName,
                    NormalizedUserName = userName!.ToUpper(),
                    Email = email,
                    NormalizedEmail = email!.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                user.PasswordHash = hasher.HashPassword(user, password!);

                UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = Roles.FirstOrDefault(r => r.Name == "Admin")?.Id! });

                Users.Add(user);
                SaveChanges();
            }
        }
    }
}