-- Health Declaration tables - run against your SQL Server database if not using EF migrations
-- Tables: CustomerHealthDeclarations, CustomerHealthDeclarationAnswers

IF OBJECT_ID(N'dbo.CustomerHealthDeclarationAnswers', N'U') IS NOT NULL
    DROP TABLE dbo.CustomerHealthDeclarationAnswers;
IF OBJECT_ID(N'dbo.CustomerHealthDeclarations', N'U') IS NOT NULL
    DROP TABLE dbo.CustomerHealthDeclarations;

CREATE TABLE dbo.CustomerHealthDeclarations (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_CustomerHealthDeclarations PRIMARY KEY,
    CustomerId NVARCHAR(64) NOT NULL,
    ContextKey NVARCHAR(100) NULL,
    HeightCm DECIMAL(9,2) NOT NULL,
    WeightKg DECIMAL(9,2) NOT NULL,
    Bmi DECIMAL(9,2) NOT NULL,
    HealthDeclaration BIT NOT NULL,
    CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_CustomerHealthDeclarations_CreatedOn DEFAULT SYSUTCDATETIME(),
    UpdatedOn DATETIME2 NULL
);

CREATE UNIQUE INDEX IX_CustomerHealthDeclarations_CustomerId_ContextKey
    ON dbo.CustomerHealthDeclarations (CustomerId, ContextKey)
    WHERE ContextKey IS NOT NULL;

CREATE TABLE dbo.CustomerHealthDeclarationAnswers (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_CustomerHealthDeclarationAnswers PRIMARY KEY,
    DeclarationId UNIQUEIDENTIFIER NOT NULL,
    Question INT NOT NULL,
    Answer BIT NOT NULL,
    Narration NVARCHAR(MAX) NULL,
    CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_CustomerHealthDeclarationAnswers_CreatedOn DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_CustomerHealthDeclarationAnswers_Declaration
        FOREIGN KEY (DeclarationId) REFERENCES dbo.CustomerHealthDeclarations(Id) ON DELETE CASCADE
);

CREATE INDEX IX_CustomerHealthDeclarationAnswers_DeclarationId
    ON dbo.CustomerHealthDeclarationAnswers (DeclarationId);

CREATE UNIQUE INDEX IX_CustomerHealthDeclarationAnswers_DeclarationId_Question
    ON dbo.CustomerHealthDeclarationAnswers (DeclarationId, Question);
