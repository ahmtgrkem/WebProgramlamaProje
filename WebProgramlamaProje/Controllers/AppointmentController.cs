using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointment
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Where(a => a.AppUserId == user.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Appointment/Create
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");

            var trainers = _context.Trainers.Select(t => new
            {
                Id = t.Id,
                FullName = t.FirstName + " " + t.LastName + " (" + t.Specialization + ")"
            }).ToList();

            ViewData["TrainerId"] = new SelectList(trainers, "Id", "FullName");

            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentDate,ServiceId,TrainerId")] Appointment appointment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            appointment.AppUserId = user.Id;
            appointment.Status = AppointmentStatus.Pending;

            // Get Service details for validation and Price
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("ServiceId", "Geçersiz hizmet seçimi.");
            }
            else
            {
                appointment.Price = service.Price;

                // Validation 1: Date in past
                if (appointment.AppointmentDate < DateTime.Now)
                {
                    ModelState.AddModelError("AppointmentDate", "Geçmiş bir tarihe randevu alamazsınız.");
                }

                // Validation 2: Conflict Check
                // New Appointment Interval: [Start, End]
                var newStart = appointment.AppointmentDate;
                var newEnd = newStart.AddMinutes(service.DurationMinutes);

                // Check Trainer's existing appointments
                var conflicts = await _context.Appointments
                    .Include(a => a.Service)
                    .Where(a => a.TrainerId == appointment.TrainerId && a.Status != AppointmentStatus.Cancelled)
                    .ToListAsync();

                foreach (var existing in conflicts)
                {
                    if (existing.Service == null) continue;

                    var existingStart = existing.AppointmentDate;
                    var existingEnd = existingStart.AddMinutes(existing.Service.DurationMinutes);

                    // Overlap condition: (StartA < EndB) and (EndA > StartB)
                    if (newStart < existingEnd && newEnd > existingStart)
                    {
                        ModelState.AddModelError("AppointmentDate", $"Seçilen saatte antrenör dolu. (Çakışan Randevu: {existingStart:HH:mm} - {existingEnd:HH:mm})");
                        break;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate lists on error
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);

            var trainers = _context.Trainers.Select(t => new
            {
                Id = t.Id,
                FullName = t.FirstName + " " + t.LastName + " (" + t.Specialization + ")"
            }).ToList();
            ViewData["TrainerId"] = new SelectList(trainers, "Id", "FullName", appointment.TrainerId);

            return View(appointment);
        }
    }
}
