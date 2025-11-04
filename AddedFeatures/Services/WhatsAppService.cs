using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace WhatsAppChatbotSystem.Services
{
    public class WhatsAppService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _token;
        private readonly string _phoneNumberId;

        public WhatsAppService(HttpClient http, string baseUrl, string token, string phoneNumberId)
        {
            _http = http;
            _baseUrl = baseUrl; // e.g. https://graph.facebook.com/v15.0
            _token = token;
            _phoneNumberId = phoneNumberId;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        // Upload media (PDF) and send it as a document to a phone number
        public async Task<string> SendPdfToUserAsync(string toPhoneNumber, string pdfFilePath, string caption = null)
        {
            // 1) Upload media
            using var content = new MultipartFormDataContent();
            var fileStream = File.OpenRead(pdfFilePath);
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            content.Add(streamContent, "file", Path.GetFileName(pdfFilePath));

            var uploadUrl = $"{_baseUrl}/{_phoneNumberId}/media";
            var uploadResp = await _http.PostAsync(uploadUrl, content);
            var uploadStr = await uploadResp.Content.ReadAsStringAsync();
            uploadResp.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(uploadStr);
            if(!doc.RootElement.TryGetProperty("id", out var idElem))
                throw new Exception("No media id returned from WhatsApp API: " + uploadStr);

            var mediaId = idElem.GetString();

            // 2) Send document message referencing mediaId
            var messageUrl = $"{_baseUrl}/{_phoneNumberId}/messages";
            var payload = new
            {
                messaging_product = "whatsapp",
                to = toPhoneNumber,
                type = "document",
                document = new { id = mediaId, filename = Path.GetFileName(pdfFilePath), caption = caption }
            };
            var payloadStr = JsonSerializer.Serialize(payload);
            var resp = await _http.PostAsync(messageUrl, new StringContent(payloadStr, System.Text.Encoding.UTF8, "application/json"));
            var respStr = await resp.Content.ReadAsStringAsync();
            resp.EnsureSuccessStatusCode();
            return respStr;
        }
    }
}
