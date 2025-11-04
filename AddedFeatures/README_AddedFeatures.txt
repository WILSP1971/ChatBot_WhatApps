WhatsAppChatbotSystem - Added Features
=====================================

Included files (AddedFeatures):
- Migration_Add_Chat_HistoriaClinica.sql : SQL script to create ChatRegistro and HistoriaClinica tables.
- ChatController.cs : API endpoint POST /api/chat/log to record chat messages into ChatRegistro.
- HistoriaClinicaController.cs : API endpoint POST /api/historiaclinica/save to store historia clinica, create a placeholder PDF and save record to DB.

Operator Panel:
- OperatorPanel/OperatorPanel.cshtml : simplified operator panel UI with chat window and a modal for the historia clínica.
  - The modal is interactive and can be used while sending messages.
  - JS uses axios to call the added APIs.

PDF Generation:
- The controller writes a placeholder PDF file. Replace with real PDF generation using QuestPDF or iTextSharp.
  - Example: add NuGet package QuestPDF and implement document layout based on Historia_Clinica.pdf provided.

Notes:
- Update your ASP.NET Core project to include the controllers and register static files (wwwroot) to serve generated PDFs.
- Ensure the connection string is valid and the database TelemedicinaBD is accessible.
- For production, adjust file paths, security, and sanitize inputs.

Generated on: 2025-11-04T20:31:07.504605 UTC


ADDITIONAL INSTRUCTIONS:
- Install QuestPDF NuGet package in your ASP.NET Core project:
  dotnet add package QuestPDF
- In Program.cs register QuestPDF license (if commercial) and optionally register WhatsAppService in DI:
  services.AddSingleton<WhatsAppService>(sp => new WhatsAppService(new HttpClient(), Environment.GetEnvironmentVariable("WHATSAPP_BASE_URL"), Environment.GetEnvironmentVariable("WHATSAPP_TOKEN"), Environment.GetEnvironmentVariable("WHATSAPP_PHONE_ID")));
- Set environment variables WHATSAPP_BASE_URL, WHATSAPP_TOKEN, WHATSAPP_PHONE_ID in your Render.com service or hosting provider.
- Build and run. Generated PDFs will be placed in wwwroot/historiaclinica and sent via WhatsApp if telefono provided.
