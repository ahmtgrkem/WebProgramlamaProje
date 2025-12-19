using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Services;

namespace WebProgramlamaProje.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly GeminiApiService _geminiService;

        public AIController(GeminiApiService geminiService)
        {
            _geminiService = geminiService;
        }

        // GET: AI
        public IActionResult Index()
        {
            return View();
        }

        // POST: AI/AnalyzePhysique
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnalyzePhysique(IFormFile imageFile, string? userNotes)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("imageFile", "Lütfen geçerli bir resim yükleyin.");
                return View("Index");
            }

            // Validate file type (simple check)
            if (!imageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("imageFile", "Sadece resim dosyaları kabul edilmektedir.");
                return View("Index");
            }

            try
            {
                string prompt = "Bu resimdeki kişinin fiziksel durumunu kabaca analiz et (vücut tipi, duruş bozukluğu vb.) ve ona özel 3 temel egzersizden oluşan bir antrenman rutini öner. Ayrıca bu rutini 6 ay boyunca uygularsa fiziksel olarak nasıl görüneceğini detaylıca betimle. Cevabı tamamen Türkçe ver. Markdown formatında başlıklar ve maddeler kullanarak düzenli bir çıktı üret.";

                if (!string.IsNullOrEmpty(userNotes))
                {
                    prompt += $" Kullanıcı Notları: {userNotes}";
                }

                string analysisResult = await _geminiService.GetWorkoutPlanFromImageAsync(imageFile, prompt);

                // Image Generation for "Future Self"
                string imagePrompt = "Transform the person in this image to look more muscular, athletic and fit, as if they have been working out for 6 months. Keep the face, pose, clothing style and background exactly the same. Photorealistic style.";
                string? generatedImageBase64 = await _geminiService.GenerateEditedImageAsync(imageFile, imagePrompt);

                ViewBag.GeneratedImage = generatedImageBase64;

                return View("Result", model: analysisResult);
            }
            catch (Exception ex)
            {
                // Log the error in a real app
                ModelState.AddModelError("", "Yapay zeka analizi sırasında bir hata oluştu: " + ex.Message);
                return View("Index");
            }
        }
    }
}
