-- Migration: Add ChatRegistro and HistoriaClinica tables
-- Run in TelemedicinaBD
CREATE TABLE ChatRegistro (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ConversationId NVARCHAR(200) NOT NULL,
    PhoneNumber NVARCHAR(50) NOT NULL,
    CustomerName NVARCHAR(200) NULL,
    MessageId NVARCHAR(200) NULL,
    Content NVARCHAR(MAX) NULL,
    MessageType NVARCHAR(50) NULL, -- Customer, Operator, Bot, System
    Sender NVARCHAR(200) NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    MediaUrl NVARCHAR(500) NULL,
    MediaType NVARCHAR(100) NULL
);

CREATE TABLE HistoriaClinica (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NoHistoria NVARCHAR(50) NOT NULL,
    NoCaso NVARCHAR(50) NULL,
    NoAdmision NVARCHAR(50) NULL,
    NoIdentificacion NVARCHAR(50) NULL,
    NombrePaciente NVARCHAR(300) NULL,
    Edad INT NULL,
    Sexo NVARCHAR(20) NULL,
    Direccion NVARCHAR(500) NULL,
    Ciudad NVARCHAR(200) NULL,
    Telefono NVARCHAR(100) NULL,
    Ocupacion NVARCHAR(200) NULL,
    EstadoCivil NVARCHAR(100) NULL,
    FechaNacimiento DATETIME2 NULL,
    FechaIngreso DATETIME2 NULL,
    HoraIngreso NVARCHAR(50) NULL,
    NombreAcompanante NVARCHAR(300) NULL,
    Parentesco NVARCHAR(200) NULL,
    MotivoConsulta NVARCHAR(MAX) NULL,
    EnfermedadActual NVARCHAR(MAX) NULL,
    Antecedentes NVARCHAR(MAX) NULL, -- JSON
    ExamenFisico NVARCHAR(MAX) NULL, -- JSON
    Diagnosticos NVARCHAR(MAX) NULL, -- JSON
    Evolucion NVARCHAR(MAX) NULL,
    Plan NVARCHAR(MAX) NULL, -- JSON
    MedicoNombre NVARCHAR(300) NULL,
    MedicoRegistro NVARCHAR(200) NULL,
    MedicoEspecialidad NVARCHAR(200) NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PdfUrl NVARCHAR(500) NULL
);

-- Indexes
CREATE INDEX IX_ChatRegistro_ConversationId ON ChatRegistro(ConversationId);
CREATE INDEX IX_HistoriaClinica_NoHistoria ON HistoriaClinica(NoHistoria);
