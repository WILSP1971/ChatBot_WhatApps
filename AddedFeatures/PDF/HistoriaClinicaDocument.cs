using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

namespace WhatsAppChatbotSystem.PDF
{
    public class HistoriaClinicaDocument : IDocument
    {
        public Controllers.HistoriaClinicaDto Data { get; }
        public HistoriaClinicaDocument(Controllers.HistoriaClinicaDto data, string noHistoria)
        {
            Data = data;
            // Ensure NoHistoria is available for printing
            Data.GetType().GetProperty("NoHistoria")?.SetValue(Data, noHistoria);
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Documento generado automáticamente");
                });
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                {
                    stack.Item().Text("HISTORIA CLÍNICA").SemiBold().FontSize(16);
                    stack.Item().Text($"No Historia: {Data.NoHistoria ?? string.Empty}").FontSize(10);

                    //stack.Item().Text($"No Historia: {{Data.GetType().GetProperty("NoHistoria")?.GetValue(Data) ?? string.Empty}}").FontSize(10);
                });
                row.ConstantColumn(100).Height(60).AlignRight().Text("Logo").FontSize(12);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingVertical(5).Text($"Nombre: {Data.NombrePaciente}");

                // column.Item().Spacing(5).Element(c =>
                // {
                //     c.Text($"Nombre: {Data.NombrePaciente}");
                // });

                column.Item().Element(c => c.Text($"Identificación: {Data.NoIdentificacion} - Edad: {Data.Edad} - Sexo: {Data.Sexo}"));

                column.Item().PaddingTop(8).Text("Motivo de consulta:").SemiBold();
                column.Item().Text(Data.MotivoConsulta ?? "-");

                column.Item().PaddingTop(6).Text("Enfermedad actual:").SemiBold();
                column.Item().Text(Data.EnfermedadActual ?? "-");

                column.Item().PaddingTop(6).Text("Antecedentes:").SemiBold();
                column.Item().Text(Data.Antecedentes != null ? System.Text.Json.JsonSerializer.Serialize(Data.Antecedentes, new System.Text.Json.JsonSerializerOptions{WriteIndented=true}) : "-");

                column.Item().PaddingTop(6).Text("Exploración física:").SemiBold();
                column.Item().Text(Data.ExamenFisico != null ? System.Text.Json.JsonSerializer.Serialize(Data.ExamenFisico, new System.Text.Json.JsonSerializerOptions{WriteIndented=true}) : "-");

                column.Item().PaddingTop(6).Text("Diagnósticos:").SemiBold();
                column.Item().Text(Data.Diagnosticos != null ? string.Join(", ", Data.Diagnosticos) : "-");

                column.Item().PaddingTop(6).Text("Plan:").SemiBold();
                column.Item().Text(Data.Plan != null ? string.Join("\n", Data.Plan) : "-");

                column.Item().PaddingTop(10).Text($"Médico: {Data.MedicoNombre} - Registro: {Data.MedicoRegistro} - Especialidad: {Data.MedicoEspecialidad}");
            });
        }
    }
}
