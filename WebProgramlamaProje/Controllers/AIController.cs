using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Services;

namespace WebProgramlamaProje.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly GeminiService _geminiService;

        public AIController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag.Error = "Lütfen analiz edilecek bir fotoğraf yükleyin.";
                return View("Index");
            }

            if (!imageFile.ContentType.StartsWith("image/"))
            {
                ViewBag.Error = "Lütfen geçerli bir resim dosyası (JPG, PNG vb.) yükleyin.";
                return View("Index");
            }

            // Dosya boyutu kontrolü (Örn: 4MB limit)
            if (imageFile.Length > 4 * 1024 * 1024)
            {
                ViewBag.Error = "Dosya boyutu çok büyük. Lütfen 4MB'dan küçük bir resim yükleyin.";
                return View("Index");
            }

            var prompt = "Bu fotoğraftaki kişinin vücut tipini analiz et, BMI tahmininde bulun ve forma girmesi için 3 temel egzersiz öner. Ayrıca bu egzersizleri yaparsa 6 ay sonra fiziksel olarak nasıl görüneceğini betimle. Cevabı Markdown formatında, başlıklar ve maddeler halinde ver.";

            var result = await _geminiService.AnalyzeImageAsync(imageFile, prompt);

            return View("Result", model: result);
        }
    }
}
