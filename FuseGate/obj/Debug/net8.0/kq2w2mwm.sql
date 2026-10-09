IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [APIUSERS] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Configuration] nvarchar(max) NULL,
    [PartnerId] nvarchar(max) NULL,
    [ConsumerKey] nvarchar(1000) NOT NULL,
    [ConsumerSecret] nvarchar(500) NOT NULL,
    [Salt] nvarchar(100) NOT NULL,
    [IsEnabled] bit NULL,
    [IpAddress] nvarchar(50) NULL,
    [HostUrl] nvarchar(50) NULL,
    [HostPort] nvarchar(50) NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [CreatedBy] nvarchar(50) NULL,
    [UpdatedBy] nvarchar(50) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [accountType] int NOT NULL,
    CONSTRAINT [PK_APIUSERS] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [beneficiaries] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] uniqueidentifier NOT NULL,
    [Surname] nvarchar(max) NULL,
    [Firstname] nvarchar(max) NULL,
    [OtherNames] nvarchar(max) NULL,
    [EmailAddress] nvarchar(max) NULL,
    [Gender] int NULL,
    [Phone] nvarchar(max) NULL,
    [Percentage] int NULL,
    [Relationship] nvarchar(max) NULL,
    [IdNumber] nvarchar(max) NULL,
    [Date_of_birth] nvarchar(max) NULL,
    [Confirmed] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [IsLocked] bit NOT NULL,
    [PartnerBeneficiaryCode] nvarchar(max) NULL,
    CONSTRAINT [PK_beneficiaries] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Customers] (
    [Id] nvarchar(450) NOT NULL,
    [PartnerId] nvarchar(max) NOT NULL,
    [ProductId] int NULL,
    [Firstname] nvarchar(max) NULL,
    [OtherNames] nvarchar(max) NULL,
    [Fullname] nvarchar(max) NULL,
    [DateOfBirth] nvarchar(max) NULL,
    [IDNumber] nvarchar(max) NULL,
    [IdType] int NULL,
    [Gender] int NULL,
    [MemberNo] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Nationality] nvarchar(max) NULL,
    [Residency] nvarchar(max) NULL,
    [Occupation] nvarchar(max) NULL,
    [EmploymentTerms] nvarchar(max) NULL,
    [TaxIdNumber] nvarchar(max) NULL,
    [Pin] nvarchar(max) NULL,
    [USAddress] nvarchar(max) NULL,
    [EmployerName] nvarchar(max) NULL,
    [BusinessName] nvarchar(max) NULL,
    [NatureOfBusiness] nvarchar(max) NULL,
    [RoleInBusiness] nvarchar(max) NULL,
    [Role] nvarchar(max) NULL,
    [SourceOfIncome] nvarchar(max) NULL,
    [AdditionalSourceOfIncome] nvarchar(max) NULL,
    [AverageIncome] float NULL,
    [Stage] int NULL,
    [Step] int NULL,
    [Complete] bit NULL,
    [Processed] bit NULL,
    [Status] nvarchar(max) NULL,
    [Validated] bit NULL,
    [ValidationErrorCount] int NULL,
    [RetryCount] int NULL,
    [ValidationErrors] nvarchar(max) NULL,
    [ValidationDate] datetime2 NULL,
    [IsGroupMember] bit NULL,
    [Signature] nvarchar(max) NULL,
    [SignaturePath] nvarchar(max) NULL,
    [PolicyPath] nvarchar(max) NULL,
    [MailSent] bit NULL,
    [MailSentOn] datetime2 NULL,
    [RecordProcessed] bit NULL,
    [Modified] bit NULL,
    [LastModified] datetime2 NULL,
    [CreatedOn] datetime2 NULL,
    [RequestDate] nvarchar(max) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Guardian] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] uniqueidentifier NOT NULL,
    [BeneficiaryId] bigint NOT NULL,
    [FirstName] nvarchar(max) NULL,
    [Surname] nvarchar(max) NULL,
    [OtherNames] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [DateOfBirth] nvarchar(max) NULL,
    [Gender] int NULL,
    [IdNumber] nvarchar(max) NULL,
    [IdType] nvarchar(max) NULL,
    [Relationship] nvarchar(max) NULL,
    [Status] nvarchar(max) NULL,
    [PartnerResponseDesc] nvarchar(max) NULL,
    [PartnerResponseCode] nvarchar(max) NULL,
    [Approved] bit NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedName] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [UpdatedName] nvarchar(max) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NULL,
    [DeletedOn] datetime2 NULL,
    [DeletedById] nvarchar(max) NULL,
    [DeletedByUser] nvarchar(max) NULL,
    CONSTRAINT [PK_Guardian] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Partners] (
    [Id] int NOT NULL IDENTITY,
    [PartnerCode] nvarchar(max) NOT NULL,
    [PartnerName] nvarchar(max) NOT NULL,
    [PartnerDescription] nvarchar(max) NOT NULL,
    [PartnerType] nvarchar(max) NULL,
    [Active] bit NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedName] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [UpdatedName] nvarchar(max) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NULL,
    [DeletedOn] datetime2 NULL,
    [DeletedById] nvarchar(max) NULL,
    [DeletedByUser] nvarchar(max) NULL,
    CONSTRAINT [PK_Partners] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [pensionerBalanceRequests] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [ProductId] bigint NOT NULL,
    [PartnerCode] nvarchar(50) NULL,
    [OtpCode] nvarchar(10) NOT NULL,
    [GeneratedOn] datetime2 NOT NULL,
    [ExpireOn] datetime2 NOT NULL,
    [AuthorisedOn] datetime2 NULL,
    [RequestRef] nvarchar(50) NOT NULL,
    [PartnerId] nvarchar(50) NOT NULL,
    [AgentCode] nvarchar(50) NOT NULL,
    [RequestCompleted] bit NOT NULL,
    [RequestFailed] bit NOT NULL,
    [RequestAuthorised] bit NOT NULL,
    CONSTRAINT [PK_pensionerBalanceRequests] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [productName] nvarchar(max) NOT NULL,
    [productenum] int NOT NULL,
    [Prefix] nvarchar(10) NOT NULL,
    [Emailtemplate] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Beneficiaries] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] nvarchar(max) NOT NULL,
    [MemberNo] nvarchar(max) NULL,
    [PartnerBeneficiaryCode] nvarchar(max) NULL,
    [Firstname] nvarchar(max) NULL,
    [OtherNames] nvarchar(max) NULL,
    [EmailAddress] nvarchar(max) NULL,
    [PartnerCode] nvarchar(max) NULL,
    [Gender] int NULL,
    [Phone] nvarchar(max) NULL,
    [Percentage] decimal(18,2) NULL,
    [Relationship] nvarchar(max) NULL,
    [IdNumber] nvarchar(max) NULL,
    [Date_of_birthRaw] nvarchar(max) NULL,
    [Date_of_birth] nvarchar(max) NULL,
    [Age] int NULL,
    [Status] nvarchar(max) NULL,
    [GuardianId] bigint NULL,
    [PartnerResponseDesc] nvarchar(max) NULL,
    [PartnerResponseCode] nvarchar(max) NULL,
    [Approved] nvarchar(max) NULL,
    [Confirmed] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [IsLocked] bit NOT NULL,
    [Surname] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedName] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [UpdatedName] nvarchar(max) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [DeletedOn] datetime2 NULL,
    [DeletedById] nvarchar(max) NULL,
    [DeletedByUser] nvarchar(max) NULL,
    CONSTRAINT [PK_Beneficiaries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Beneficiaries_Guardian_GuardianId] FOREIGN KEY ([GuardianId]) REFERENCES [Guardian] ([Id])
);
GO

CREATE TABLE [PartnersProducts] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(100) NULL,
    [PartnerId] int NULL,
    [ProductId] int NULL,
    [Image] nvarchar(50) NULL,
    [Active] bit NOT NULL,
    [CreatedOn] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_PartnersProducts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PartnersProducts_Partners_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Partners] ([Id]),
    CONSTRAINT [FK_PartnersProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id])
);
GO

CREATE TABLE [gurdians] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] uniqueidentifier NULL,
    [BeneficiaryId] bigint NOT NULL,
    [Surname] nvarchar(max) NULL,
    [OtherNames] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [DOB] nvarchar(max) NULL,
    [Gender] int NULL,
    [IdNumber] nvarchar(max) NULL,
    [IdType] nvarchar(max) NULL,
    [Relationship] nvarchar(max) NULL,
    [GuardianCode] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_gurdians] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_gurdians_Beneficiaries_BeneficiaryId] FOREIGN KEY ([BeneficiaryId]) REFERENCES [Beneficiaries] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [msureRequests] (
    [Id] nvarchar(450) NOT NULL,
    [PartnersId] int NOT NULL,
    [ProductsId] int NULL,
    [customerId] nvarchar(max) NULL,
    [benefitOption] int NULL,
    [transactionId] nvarchar(max) NULL,
    [optinTime] nvarchar(max) NULL,
    [Customername] nvarchar(max) NULL,
    [Gender] nvarchar(max) NULL,
    [premium] float NULL,
    [status] nvarchar(max) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [Processed] bit NOT NULL,
    CONSTRAINT [PK_msureRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_msureRequests_PartnersProducts_ProductsId] FOREIGN KEY ([ProductsId]) REFERENCES [PartnersProducts] ([Id]),
    CONSTRAINT [FK_msureRequests_Partners_PartnersId] FOREIGN KEY ([PartnersId]) REFERENCES [Partners] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Beneficiaries_GuardianId] ON [Beneficiaries] ([GuardianId]);
GO

CREATE INDEX [IX_gurdians_BeneficiaryId] ON [gurdians] ([BeneficiaryId]);
GO

CREATE INDEX [IX_msureRequests_PartnersId] ON [msureRequests] ([PartnersId]);
GO

CREATE INDEX [IX_msureRequests_ProductsId] ON [msureRequests] ([ProductsId]);
GO

CREATE INDEX [IX_PartnersProducts_PartnerId] ON [PartnersProducts] ([PartnerId]);
GO

CREATE INDEX [IX_PartnersProducts_ProductId] ON [PartnersProducts] ([ProductId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260302092601_pensionerInfo', N'7.0.3');
GO

COMMIT;
GO

