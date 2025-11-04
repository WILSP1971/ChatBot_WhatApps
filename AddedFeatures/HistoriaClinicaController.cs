using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using WhatsAppChatbotSystem.PDF;
using WhatsAppChatbotSystem.Services;
using System.Net.Http;

namespace WhatsAppChatbotSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoriaClinicaController : ControllerBase
    {
        private readonly string _conn = "Server=localhost;Database=TelemedicinaBD;User Id=sa;Password=ms2022*;TrustServerCertificate=True;MultipleActiveResultSets=true";
        private readonly WhatsAppService _whatsAppService;

        public HistoriaClinicaController()
        {
            // In production register WhatsAppService via DI. For now create a simple instance with placeholders.
            var http = new HttpClient();
            var baseUrl = Environment.GetEnvironmentVariable("WHATSAPP_BASE_URL") ?? "https://graph.facebook.com/v15.0";
            var token = Environment.GetEnvironmentVariable("WHATSAPP_TOKEN") ?? "YOUR_WHATSAPP_TOKEN";
            var phoneId = Environment.GetEnvironmentVariable("WHATSAPP_PHONE_ID") ?? "YOUR_PHONE_NUMBER_ID";
            _whatsAppService = new WhatsAppService(http, baseUrl, token, phoneId);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveHistoria([FromBody] HistoriaClinicaDto dto)
        {
            var noHistoria = "HC-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var pdfFileName = $"Historia_{noHistoria}.pdf";
            var pdfRelativePath = Path.Combine("historiaclinica", pdfFileName);
            var wwwroot = Path.Combine(Environment.CurrentDirectory, "wwwroot");
            var pdfPath = Path.Combine(wwwroot, pdfRelativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));

            // Prepare JSON fields
            var antecedentesJson = JsonSerializer.Serialize(dto.Antecedentes);
            var examenFisicoJson = JsonSerializer.Serialize(dto.ExamenFisico);
            var diagnosticosJson = JsonSerializer.Serialize(dto.Diagnosticos);
            var planJson = JsonSerializer.Serialize(dto.Plan);

            using (var conn = new SqlConnection(_conn))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
INSERT INTO HistoriaClinica (NoHistoria, NoCaso, NoAdmision, NoIdentificacion, NombrePaciente, Edad, Sexo, Direccion, Ciudad, Telefono, Ocupacion, EstadoCivil, FechaNacimiento, FechaIngreso, HoraIngreso, NombreAcompanante, Parentesco, MotivoConsulta, EnfermedadActual, Antecedentes, ExamenFisico, Diagnosticos, Evolucion, Plan, MedicoNombre, MedicoRegistro, MedicoEspecialidad, FechaCreacion, PdfUrl)
VALUES (@NoHistoria, @NoCaso, @NoAdmision, @NoIdentificacion, @NombrePaciente, @Edad, @Sexo, @Direccion, @Ciudad, @Telefono, @Ocupacion, @EstadoCivil, @FechaNacimiento, @FechaIngreso, @HoraIngreso, @NombreAcompanante, @Parentesco, @MotivoConsulta, @EnfermedadActual, @Antecedentes, @ExamenFisico, @Diagnosticos, @Evolucion, @Plan, @MedicoNombre, @MedicoRegistro, @MedicoEspecialidad, @FechaCreacion, @PdfUrl);
SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@NoHistoria", noHistoria);
                cmd.Parameters.AddWithValue("@NoCaso", (object)dto.NoCaso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NoAdmision", (object)dto.NoAdmision ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NoIdentificacion", (object)dto.NoIdentificacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NombrePaciente", (object)dto.NombrePaciente ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Edad", (object)dto.Edad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", (object)dto.Sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Direccion", (object)dto.Direccion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ciudad", (object)dto.Ciudad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object)dto.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Ocupacion", (object)dto.Ocupacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EstadoCivil", (object)dto.EstadoCivil ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaNacimiento", (object)dto.FechaNacimiento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaIngreso", (object)dto.FechaIngreso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HoraIngreso", (object)dto.HoraIngreso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NombreAcompanante", (object)dto.NombreAcompanante ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Parentesco", (object)dto.Parentesco ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotivoConsulta", (object)dto.MotivoConsulta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EnfermedadActual", (object)dto.EnfermedadActual ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Antecedentes", (object)antecedentesJson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ExamenFisico", (object)examenFisicoJson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Diagnosticos", (object)diagnosticosJson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Evolucion", (object)dto.Evolucion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Plan", (object)planJson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MedicoNombre", (object)dto.MedicoNombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MedicoRegistro", (object)dto.MedicoRegistro ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MedicoEspecialidad", (object)dto.MedicoEspecialidad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@PdfUrl", (object)pdfRelativePath ?? DBNull.Value);

                await conn.OpenAsync();
                var idObj = await cmd.ExecuteScalarAsync();
            }

            // Generate PDF using QuestPDF
            try
            {
                var document = new HistoriaClinicaDocument(dto, noHistoria);
                var bytes = document.GeneratePdf();
                await System.IO.File.WriteAllBytesAsync(pdfPath, bytes);
            }
            catch(Exception ex)
            {
                // If QuestPDF not installed, fallback to placeholder text file renamed as pdf
                System.IO.File.WriteAllText(pdfPath, "PDF generation failed. Install QuestPDF NuGet. Exception: " + ex.Message);
            }

            // Send PDF via WhatsApp if phone number provided
            if(!string.IsNullOrEmpty(dto.Telefono))
            {
                try
                {
                    // WhatsApp requires international format without plus for some APIs, adjust accordingly
                    var toNumber = dto.Telefono.Replace("+", "").Replace(" ", "");
                    var sendResult = await _whatsAppService.SendPdfToUserAsync(toNumber, pdfPath, "Historia clínica adjunta: " + noHistoria);
                }
                catch(Exception ex)
                {
                    // Log send failure (in production replace with proper logging)
                }
            }

            var response = new { noHistoria = noHistoria, pdfUrl = "/" + pdfRelativePath.Replace("\\","/") };
            return Ok(new { success = true, data = response });
        }
    }

    // Extend DTO with NoHistoria property for printing convenience
    public partial class HistoriaClinicaDto
    {
        public string NoHistoria { get; set; }
    }
}
