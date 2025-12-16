using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;

namespace WebProgramlamaProje.Controllers
{
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Service
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.Include(s => s.Gym).ToListAsync();
            return View(services);
        }

        // GET: Service/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.Gym)
                .Include(s => s.Trainers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }
    }
}
