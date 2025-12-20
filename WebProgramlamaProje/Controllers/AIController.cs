using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Services;
using WebProgramlamaProje.ViewModels;

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

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnalyzePhysique(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("imageFile", "Lütfen geçerli bir resim yükleyin.");
                return View("Index");
            }

            if (!imageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("imageFile", "Sadece resim dosyaları kabul edilmektedir.");
                return View("Index");
            }

            try
            {
                string uploadedImageBase64;
                using (var ms = new MemoryStream())
                {
                    await imageFile.CopyToAsync(ms);
                    uploadedImageBase64 = $"data:{imageFile.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
                }

                // 1. Analyze Physique
                string analysisPrompt = "Bu vücut tipini kısaca analiz et (yağ oranı, kas kütlesi). Çok kısa, özet bir antrenman ve beslenme tavsiyesi ver. Uzun açıklamalardan kaçın, madde madde ve net ol.";
                string analysisResult = await _geminiService.GetWorkoutPlanFromImageAsync(imageFile, analysisPrompt);

                // 2. Generate Image Prompt (Simplified for now)
                // In a real app, we might parse the analysisResult to get specific body type details.
                string imageGenPrompt = "Keep the exact same person, face, pose, clothing and background. Only slightly increase muscle mass and definition to show an athletic physique. Do not change the lighting or environment. Photorealistic.";

                // 3. Generate Target Body Image (Using the uploaded image as base)
                string generatedImageResult = await _geminiService.GenerateEditedImageAsync(imageFile, imageGenPrompt);

                string? generatedImageBase64 = null;
                if (!generatedImageResult.StartsWith("ERROR:") && !generatedImageResult.StartsWith("HATA:") && !generatedImageResult.StartsWith("UYARI:"))
                {
                    generatedImageBase64 = generatedImageResult;
                }
                else
                {
                    // Append error to analysis for debugging (optional, or handle silently)
                    analysisResult += $"\n\n---\n[Sistem Bilgisi: Görsel oluşturulamadı. Hata: {generatedImageResult}]";
                }

                // 4. Prepare ViewModel
                var viewModel = new AIResultViewModel
                {
                    AnalysisResult = analysisResult,
                    GeneratedImageBase64 = generatedImageBase64 ?? "",
                    UploadedImageBase64 = uploadedImageBase64
                };

                return View("Result", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
                return View("Index");
            }
        }
    }
}
