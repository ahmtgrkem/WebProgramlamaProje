using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                if (context == null || userManager == null || roleManager == null) return;

                // Veritabanını güncelle (Migration'ları uygula)
                await context.Database.MigrateAsync();

                // 1. Gym (Spor Salonu)
                if (!await context.Gyms.AnyAsync())
                {
                    var gym = new Gym
                    {
                        Name = "Sakarya FitLife Center",
                        Address = "Esentepe Kampüsü, Serdivan/Sakarya",
                        ContactNumber = "0264 123 45 67",
                        OpeningHours = "Hafta İçi: 07:00 - 23:00, Hafta Sonu: 09:00 - 21:00"
                    };
                    await context.Gyms.AddAsync(gym);
                    await context.SaveChangesAsync();
                }

                var gymEntity = await context.Gyms.FirstOrDefaultAsync();

                // 2. Services (Hizmetler)
                if (!await context.Services.AnyAsync())
                {
                    var services = new List<Service>
                    {
                        new Service { Name = "Birebir Fitness Antrenman", DurationMinutes = 60, Price = 500, Description = "Kişisel antrenör eşliğinde özel program.", GymId = gymEntity.Id },
                        new Service { Name = "Grup Yoga", DurationMinutes = 45, Price = 200, Description = "Esneklik ve zihin rahatlığı için grup dersi.", GymId = gymEntity.Id },
                        new Service { Name = "Pilates Reformer", DurationMinutes = 50, Price = 400, Description = "Aletli pilates ile duruş bozukluklarını düzeltin.", GymId = gymEntity.Id },
                        new Service { Name = "Kick Boks", DurationMinutes = 60, Price = 350, Description = "Yüksek tempolu dövüş sanatları antrenmanı.", GymId = gymEntity.Id }
                    };
                    await context.Services.AddRangeAsync(services);
                    await context.SaveChangesAsync();
                }

                // 3. Trainers (Eğitmenler)
                if (!await context.Trainers.AnyAsync())
                {
                    var services = await context.Services.ToListAsync();

                    // Helper function to get service by name safely
                    Service GetService(string namePart) => services.FirstOrDefault(s => s.Name.Contains(namePart)) ?? services.First();

                    var trainers = new List<Trainer>
                    {
                        new Trainer {
                            FirstName = "Ahmet",
                            LastName = "Yılmaz",
                            Specialization = "Fitness & Vücut Geliştirme",
                            IsAvailable = true,
                            AvailabilityDescription = "Hafta içi her gün",
                            Services = new List<Service> { GetService("Fitness") }
                        },
                        new Trainer {
                            FirstName = "Ayşe",
                            LastName = "Demir",
                            Specialization = "Yoga & Pilates",
                            IsAvailable = true,
                            AvailabilityDescription = "Salı ve Perşembe günleri",
                            Services = new List<Service> { GetService("Yoga"), GetService("Pilates") }
                        },
                        new Trainer {
                            FirstName = "Mehmet",
                            LastName = "Kaya",
                            Specialization = "Kick Boks & Kondisyon",
                            IsAvailable = false,
                            AvailabilityDescription = "Şu an izinde",
                            Services = new List<Service> { GetService("Kick Boks") }
                        },
                        new Trainer {
                            FirstName = "Zeynep",
                            LastName = "Çelik",
                            Specialization = "Reformer Pilates",
                            IsAvailable = true,
                            AvailabilityDescription = "Hafta sonları",
                            Services = new List<Service> { GetService("Pilates") }
                        },
                        new Trainer {
                            FirstName = "Can",
                            LastName = "Öztürk",
                            Specialization = "Fonksiyonel Antrenman",
                            IsAvailable = true,
                            AvailabilityDescription = "Akşam saatleri",
                            Services = new List<Service> { GetService("Fitness") }
                        }
                    };
                    await context.Trainers.AddRangeAsync(trainers);
                    await context.SaveChangesAsync();
                }

                // 4. Admin User (Yönetici)
                var adminEmail = "g231210032@ogr.sakarya.edu.tr";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Görkem",
                        LastName = "Admin",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, "Sau.123!");

                    if (result.Succeeded)
                    {
                        if (!await roleManager.RoleExistsAsync("Admin"))
                        {
                            await roleManager.CreateAsync(new IdentityRole("Admin"));
                        }
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // 5. Test Member User (Test Üyesi)
                var testUserEmail = "test@uye.com";
                var testUser = await userManager.FindByEmailAsync(testUserEmail);
                if (testUser == null)
                {
                    testUser = new AppUser
                    {
                        UserName = testUserEmail,
                        Email = testUserEmail,
                        FirstName = "Test",
                        LastName = "Üye",
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(testUser, "Sau.123!");

                    if (result.Succeeded)
                    {
                        // Rol kontrolü ve atama
                        if (!await roleManager.RoleExistsAsync("Member"))
                        {
                            await roleManager.CreateAsync(new IdentityRole("Member"));
                        }
                        await userManager.AddToRoleAsync(testUser, "Member");
                    }
                }

                // 5. Appointments (Randevular)
                if (!await context.Appointments.AnyAsync())
                {
                    var trainer = await context.Trainers.FirstOrDefaultAsync(t => t.FirstName == "Ahmet");
                    var service = await context.Services.FirstOrDefaultAsync(s => s.Name.Contains("Fitness"));

                    if (trainer != null && service != null && testUser != null)
                    {
                        var appointments = new List<Appointment>
                        {
                            // Geçmiş Randevu (Tamamlanmış)
                            new Appointment
                            {
                                AppointmentDate = DateTime.Now.AddDays(-5),
                                Status = AppointmentStatus.Completed,
                                Price = service.Price,
                                AppUserId = testUser.Id,
                                TrainerId = trainer.Id,
                                ServiceId = service.Id
                            },
                            // Gelecek Randevu (Beklemede)
                            new Appointment
                            {
                                AppointmentDate = DateTime.Now.AddDays(2).AddHours(10), // 2 gün sonra saat 10:00
                                Status = AppointmentStatus.Pending,
                                Price = service.Price,
                                AppUserId = testUser.Id,
                                TrainerId = trainer.Id,
                                ServiceId = service.Id
                            }
                        };
                        await context.Appointments.AddRangeAsync(appointments);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
