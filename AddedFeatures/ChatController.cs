using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Collections.Generic;

namespace WhatsAppChatbotSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly string _conn = "Server=localhost;Database=TelemedicinaBD;User Id=sa;Password=ms2022*;TrustServerCertificate=True;MultipleActiveResultSets=true";

        [HttpPost("log")]
        public async Task<IActionResult> LogChat([FromBody] ChatRegistroModel model)
        {
            // Basic validation
            if (string.IsNullOrEmpty(model.ConversationId) || string.IsNullOrEmpty(model.PhoneNumber))
                return BadRequest(new { success = false, message = "ConversationId and PhoneNumber required" });

            using (var conn = new SqlConnection(_conn))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO ChatRegistro (ConversationId, PhoneNumber, CustomerName, MessageId, Content, MessageType, Sender, Timestamp, MediaUrl, MediaType)
VALUES (@ConversationId, @PhoneNumber, @CustomerName, @MessageId, @Content, @MessageType, @Sender, @Timestamp, @MediaUrl, @MediaType)";
                cmd.Parameters.AddWithValue("@ConversationId", model.ConversationId);
                cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                cmd.Parameters.AddWithValue("@CustomerName", (object)model.CustomerName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MessageId", (object)model.MessageId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Content", (object)model.Content ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MessageType", (object)model.MessageType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sender", (object)model.Sender ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Timestamp", model.Timestamp == default ? DateTime.UtcNow : model.Timestamp);
                cmd.Parameters.AddWithValue("@MediaUrl", (object)model.MediaUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MediaType", (object)model.MediaType ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok(new { success = true });
        }
    }

    public class ChatRegistroModel
    {
        public string ConversationId { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string MessageId { get; set; } = "";
        public string Content { get; set; } = "";
        public string MessageType { get; set; } = "";
        public string Sender { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
    }
}
