using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Roles
            var adminRoleId = "c7b013f0-5201-4317-abd8-c211f91b7330";
            var memberRoleId = "fab4fac1-c546-41de-aebc-a14da6895711";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = memberRoleId, Name = "Member", NormalizedName = "MEMBER" }
            );

            // Seed Admin User
            var adminUserId = "b74ddd14-6340-4840-95c2-db12554843e5";
            var adminUser = new AppUser
            {
                Id = adminUserId,
                UserName = "ogrencinumarasi@sakarya.edu.tr",
                NormalizedUserName = "OGRENCINUMARASI@SAKARYA.EDU.TR",
                Email = "ogrencinumarasi@sakarya.edu.tr",
                NormalizedEmail = "OGRENCINUMARASI@SAKARYA.EDU.TR",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var hasher = new PasswordHasher<AppUser>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Sau.123!");

            builder.Entity<AppUser>().HasData(adminUser);

            // Assign Admin Role to Admin User
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );
        }
    }
}
