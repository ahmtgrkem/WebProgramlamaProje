using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebProgramlamaProje.Services
{
    public class GeminiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ModelName = "gemini-2.5-flash"; // gemini-2.5-flash is not yet widely available/stable in public API, falling back to 1.5-flash which is current SOTA for speed/vision. If 2.5 is strictly required, change this string.
        private const string ImageModelName = "gemini-3-pro-image-preview"; // Model for image generation/editing

        public GeminiApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key is missing in configuration.");
        }

        public async Task<string?> GenerateEditedImageAsync(IFormFile imageFile, string prompt)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file.");

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
                                inline_data = new
                                {
                                    mime_type = imageFile.ContentType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    responseModalities = new[] { "IMAGE" } // Force image output
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ImageModelName}:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(url, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                // Log error or handle gracefully. For now, return null so the main flow doesn't break.
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Image Gen Error: {error}");
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);

            var part = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault();

            if (part?.InlineData != null)
            {
                return part.InlineData.Data;
            }

            return null;
        }

        public async Task<string> GetWorkoutPlanFromImageAsync(IFormFile imageFile, string userPrompt)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file.");

            // Convert image to Base64
            string base64Image;
            using (var ms = new MemoryStream())
            {
                await imageFile.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            // Prepare Request Payload
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
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent?key={_apiKey}";

            // Send Request
            var response = await _httpClient.PostAsync(url, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error: {response.StatusCode} - {errorContent}");
            }

            // Parse Response
            var responseString = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);

            return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                   ?? "No analysis could be generated.";
        }

        // DTO Classes for JSON Deserialization
        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate>? Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content? Content { get; set; }
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

            [JsonPropertyName("inline_data")]
            public InlineData? InlineData { get; set; }
        }

        private class InlineData
        {
            [JsonPropertyName("mime_type")]
            public string? MimeType { get; set; }

            [JsonPropertyName("data")]
            public string? Data { get; set; }
        }
    }
}
