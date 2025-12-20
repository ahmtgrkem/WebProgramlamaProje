using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebProgramlamaProje.Services
{
    public class GeminiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _analysisApiKey;
        private readonly string _imageApiKey;

        // Modeller
        private const string AnalysisModelName = "gemini-2.5-flash";
        private const string ImageModelName = "gemini-3-pro-image-preview";

        public GeminiApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _analysisApiKey = configuration["Gemini:AnalysisApiKey"] ?? throw new ArgumentNullException("Gemini:AnalysisApiKey eksik.");
            _imageApiKey = configuration["Gemini:ImageApiKey"] ?? throw new ArgumentNullException("Gemini:ImageApiKey eksik.");
        }

        public async Task<string> GetWorkoutPlanFromImageAsync(IFormFile imageFile, string userPrompt)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Geçersiz resim dosyası.");

            string base64Image;
            using (var ms = new MemoryStream())
            {
                await imageFile.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = userPrompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = imageFile.ContentType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{AnalysisModelName}:generateContent?key={_analysisApiKey}";

            var response = await _httpClient.PostAsync(url, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini Analiz API Hatası: {response.StatusCode} - {errorContent}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);

            return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                   ?? "Analiz oluşturulamadı.";
        }

        public async Task<string> GenerateTargetBodyImageAsync(string imageDescription)
        {
            // PROMPT OPTİMİZASYONU:
            // "Realistic photo" yerine "Fitness illustration" veya "Professional shot" gibi terimler 
            // güvenlik filtrelerini aşmaya yardımcı olabilir. Promptun başına ekliyoruz.
            var safePrompt = $"A high quality, professional fitness photo. {imageDescription}. Lighting studio, 8k resolution.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = safePrompt }
                        }
                    }
                },
                generationConfig = new
                {
                    responseModalities = new[] { "IMAGE" }
                },
                // KRİTİK GÜVENLİK AYARLARI:
                // İnsan figürlerinin engellenmemesi için filtreleri gevşetiyoruz.
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_ONLY_HIGH" }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ImageModelName}:generateContent?key={_imageApiKey}";

            var response = await _httpClient.PostAsync(url, jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Gemini Image API Error: {response.StatusCode} - {responseString}");
                return $"ERROR: API Hatası {response.StatusCode} - Detay: {responseString}";
            }

            try
            {
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);
                var candidate = geminiResponse?.Candidates?.FirstOrDefault();
                var part = candidate?.Content?.Parts?.FirstOrDefault();

                // 1. Resim verisi var mı kontrol et
                if (part?.InlineData != null && !string.IsNullOrEmpty(part.InlineData.Data))
                {
                    // Data: "data:image/jpeg;base64,....." formatında gelmeyebilir, sadece base64 gelir.
                    // HTML'de göstermek için başına prefix ekliyoruz.
                    return $"data:{part.InlineData.MimeType ?? "image/jpeg"};base64,{part.InlineData.Data}";
                }

                // 2. Metin döndü mü? (Bazen hata mesajı metin olarak döner)
                if (!string.IsNullOrEmpty(part?.Text))
                {
                    return $"UYARI: Model resim yerine metin döndü: {part.Text}";
                }

                // 3. Hata Analizi
                if (!string.IsNullOrEmpty(candidate?.FinishReason))
                {
                    // FinishReason: STOP ise ama veri yoksa, genelde filtredir veya boş dönmüştür.
                    return $"HATA: Görsel oluşturulamadı. Sebep: {candidate.FinishReason}. (Ham Yanıt: {responseString})";
                }

                return "HATA: Beklenmeyen yanıt formatı.";
            }
            catch (Exception ex)
            {
                return $"HATA: JSON Parse hatası: {ex.Message}";
            }
        }

        public async Task<string> GenerateEditedImageAsync(IFormFile imageFile, string prompt)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Geçersiz resim dosyası.");

            string base64Image;
            using (var ms = new MemoryStream())
            {
                await imageFile.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = imageFile.ContentType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    responseModalities = new[] { "IMAGE" }
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_ONLY_HIGH" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_ONLY_HIGH" }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ImageModelName}:generateContent?key={_imageApiKey}";

            var response = await _httpClient.PostAsync(url, jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"ERROR: API Hatası {response.StatusCode} - Detay: {responseString}";
            }

            try
            {
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);
                var candidate = geminiResponse?.Candidates?.FirstOrDefault();
                var part = candidate?.Content?.Parts?.FirstOrDefault();

                if (part?.InlineData != null && !string.IsNullOrEmpty(part.InlineData.Data))
                {
                    return $"data:{part.InlineData.MimeType ?? "image/jpeg"};base64,{part.InlineData.Data}";
                }

                if (!string.IsNullOrEmpty(part?.Text))
                {
                    return $"UYARI: Model resim yerine metin döndü: {part.Text}";
                }

                if (!string.IsNullOrEmpty(candidate?.FinishReason))
                {
                    return $"HATA: Görsel oluşturulamadı. Sebep: {candidate.FinishReason}. (Ham Yanıt: {responseString})";
                }

                return "HATA: Beklenmeyen yanıt formatı.";
            }
            catch (Exception ex)
            {
                return $"HATA: JSON Parse hatası: {ex.Message}";
            }
        }

        // DTO Sınıfları
        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate>? Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content? Content { get; set; }

            [JsonPropertyName("finishReason")]
            public string? FinishReason { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public List<Part>? Parts { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }

            [JsonPropertyName("inlineData")]
            public InlineData? InlineData { get; set; }
        }

        private class InlineData
        {
            [JsonPropertyName("mimeType")]
            public string? MimeType { get; set; }

            [JsonPropertyName("data")]
            public string? Data { get; set; }
        }
    }
}