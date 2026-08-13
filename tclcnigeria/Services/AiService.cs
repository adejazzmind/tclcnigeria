using System.Text;
using System.Text.Json;

namespace tclcnigeria.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _http;
        private readonly ILogger<AiService> _logger;
        private readonly string _apiKey;

        private const string Model = "gemini-3.1-flash-lite";
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

        public AiService(HttpClient http, IConfiguration config, ILogger<AiService> logger)
        {
            _http = http;
            _logger = logger;
            _apiKey = config["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");
        }

        public async Task<string> AskAsync(string userQuestion, string contextBlock)
        {
            var systemPrompt =
                "You are the friendly AI assistant for TCLC Nigeria, a church. " +
                "Answer visitor questions warmly and briefly using ONLY the church information " +
                "provided below. If the answer isn't in the provided information, say you're not " +
                "sure and suggest they contact the church office, instead of guessing.\n\n" +
                "CHURCH INFORMATION:\n" + contextBlock;

            return await GenerateAsync(systemPrompt, userQuestion, maxOutputTokens: 400);
        }

        public async Task<string> SummarizeSermonAsync(string sermonTitle, string sermonText)
        {
            var systemPrompt =
                "You summarize church sermons for a website. Write a short summary (3-4 sentences) " +
                "followed by 3-5 bullet-point key takeaways. Keep the tone warm and clear. " +
                "Do not invent scripture references or claims that aren't in the source text.";

            var userPrompt = $"Sermon title: {sermonTitle}\n\nSermon content:\n{sermonText}";

            return await GenerateAsync(systemPrompt, userPrompt, maxOutputTokens: 500);
        }

        public async Task<string> TriagePrayerRequestAsync(string prayerText)
        {
            var systemPrompt =
                "You triage prayer requests for church staff. Read the request and respond in " +
                "EXACTLY this format, nothing else:\n" +
                "Category: <one of Health, Family, Finance, Spiritual Growth, Grief/Loss, Other>\n" +
                "Urgency: <Low, Medium, or High>\n" +
                "Note: <one short sentence summarizing the need for staff who are scanning a list>";

            return await GenerateAsync(systemPrompt, prayerText, maxOutputTokens: 150);
        }

        public async Task<string> DraftDescriptionAsync(string rawNotes)
        {
            var systemPrompt =
                "You help church admin staff turn rough notes into a polished, warm event or " +
                "sermon description suitable for a public church website. Keep it to one short " +
                "paragraph. Do not add fake details, dates, or names that weren't in the notes.";

            return await GenerateAsync(systemPrompt, rawNotes, maxOutputTokens: 250);
        }

        private async Task<string> GenerateAsync(string systemPrompt, string userPrompt, int maxOutputTokens)
        {
            var url = $"{BaseUrl}/{Model}:generateContent";

            var payload = new
            {
                system_instruction = new { parts = new[] { new { text = systemPrompt } } },
                contents = new[] { new { role = "user", parts = new[] { new { text = userPrompt } } } },
                generationConfig = new { maxOutputTokens, temperature = 0.4 }
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
                _http.DefaultRequestHeaders.Remove("x-goog-api-key");
                _http.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);

            try
            {
                using var response = await _http.PostAsync(url, content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API error {Status}: {Body}", response.StatusCode, body);

                    if ((int)response.StatusCode == 429)
                        return "I'm getting a lot of questions right now — please try again in a moment, or contact the church office directly.";

                    return "Sorry, I'm having trouble answering right now. Please try again shortly.";
                }

                using var doc = JsonDocument.Parse(body);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text?.Trim() ?? "Sorry, I couldn't generate a response just now.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API call failed.");
                return "Sorry, something went wrong. Please try again shortly.";
            }
        }
    }
}


