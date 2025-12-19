using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint 1: Gelir Raporu (Bu ayın toplam cirosu)
        [HttpGet("GetMonthlyIncome")]
        public async Task<IActionResult> GetMonthlyIncome()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var totalIncome = await _context.Appointments
                .Where(a => a.AppointmentDate.Month == currentMonth && a.AppointmentDate.Year == currentYear)
                .SumAsync(a => a.Price);

            return Ok(new
            {
                Period = $"{currentMonth}/{currentYear}",
                TotalIncome = totalIncome
            });
        }

        // Endpoint 2: Popüler Eğitmenler (En çok randevu alınan ilk 3 eğitmen)
        [HttpGet("GetTopTrainers")]
        public async Task<IActionResult> GetTopTrainers()
        {
            var topTrainers = await _context.Appointments
                .Include(a => a.Trainer)
                .GroupBy(a => a.Trainer)
                .Select(g => new
                {
                    TrainerName = g.Key.FirstName + " " + g.Key.LastName,
                    AppointmentCount = g.Count()
                })
                .OrderByDescending(x => x.AppointmentCount)
                .Take(3)
                .ToListAsync();

            return Ok(topTrainers);
        }

        // Endpoint 3: Hizmet Kullanımı (Hangi hizmetten kaç tane satıldı)
        [HttpGet("GetServiceStats")]
        public async Task<IActionResult> GetServiceStats()
        {
            var serviceStats = await _context.Appointments
                .Include(a => a.Service)
                .GroupBy(a => a.Service)
                .Select(g => new
                {
                    ServiceName = g.Key.Name,
                    SalesCount = g.Count()
                })
                .ToListAsync();

            return Ok(serviceStats);
        }
    }
}
