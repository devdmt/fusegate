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

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
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

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [APIUSERS] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NULL,
    [Configuration] nvarchar(50) NULL,
    [PartnerId] int NOT NULL,
    [ConsumerKey] nvarchar(100) NOT NULL,
    [ConsumerSecret] nvarchar(500) NOT NULL,
    [Salt] nvarchar(100) NOT NULL,
    [IsEnabled] bit NOT NULL,
    [IpAddress] nvarchar(50) NULL,
    [HostUrl] nvarchar(50) NULL,
    [HostPort] nvarchar(50) NULL,
    [CreatedBy] nvarchar(50) NULL,
    [UpdatedBy] nvarchar(50) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_APIUSERS] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_APIUSERS_Partners_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Partners] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [partnersProducts] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(100) NULL,
    [PartnerId] int NULL,
    [Image] nvarchar(max) NULL,
    [Active] bit NOT NULL,
    [CreatedOn] datetime2 NULL,
    [CreatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_partnersProducts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_partnersProducts_Partners_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Partners] ([Id])
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_APIUSERS_UserId] FOREIGN KEY ([UserId]) REFERENCES [APIUSERS] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_APIUSERS_UserId] FOREIGN KEY ([UserId]) REFERENCES [APIUSERS] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_APIUSERS_UserId] FOREIGN KEY ([UserId]) REFERENCES [APIUSERS] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_APIUSERS_UserId] FOREIGN KEY ([UserId]) REFERENCES [APIUSERS] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MsureRequests] (
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
    CONSTRAINT [PK_MsureRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MsureRequests_Partners_PartnersId] FOREIGN KEY ([PartnersId]) REFERENCES [Partners] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MsureRequests_partnersProducts_ProductsId] FOREIGN KEY ([ProductsId]) REFERENCES [partnersProducts] ([Id])
);
GO

CREATE TABLE [OnboardingRequests] (
    [Id] nvarchar(450) NOT NULL,
    [TransactionId] nvarchar(max) NOT NULL,
    [PartnerId] int NOT NULL,
    [ProductId] int NULL,
    [CustomerName] nvarchar(max) NULL,
    [DateOfBirth] nvarchar(max) NULL,
    [IDNumber] nvarchar(max) NULL,
    [Gender] int NULL,
    [Premium] float NULL,
    [BenefitOption] int NULL,
    [BeneficiaryName] nvarchar(max) NULL,
    [RegNumber] nvarchar(max) NULL,
    [BeneficiaryMobileNumber] nvarchar(max) NULL,
    [CreatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_OnboardingRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OnboardingRequests_Partners_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Partners] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OnboardingRequests_partnersProducts_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [partnersProducts] ([Id])
);
GO

CREATE INDEX [EmailIndex] ON [APIUSERS] ([NormalizedEmail]);
GO

CREATE INDEX [IX_APIUSERS_PartnerId] ON [APIUSERS] ([PartnerCode]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [APIUSERS] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [IX_MsureRequests_PartnersId] ON [MsureRequests] ([PartnersId]);
GO

CREATE INDEX [IX_MsureRequests_ProductsId] ON [MsureRequests] ([ProductsId]);
GO

CREATE INDEX [IX_OnboardingRequests_PartnerId] ON [OnboardingRequests] ([PartnerCode]);
GO

CREATE INDEX [IX_OnboardingRequests_ProductId] ON [OnboardingRequests] ([ProductId]);
GO

CREATE INDEX [IX_partnersProducts_PartnerId] ON [partnersProducts] ([PartnerCode]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240110133136_Initial', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OnboardingRequests] ADD [Processed] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [OnboardingRequests] ADD [Status] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240110133816_Initial1', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [claimRequests] (
    [Id] bigint NOT NULL IDENTITY,
    [PartnerID] nvarchar(max) NULL,
    [ProductID] nvarchar(max) NULL,
    [CustomerName] nvarchar(max) NULL,
    [ClaimRefNumber] nvarchar(max) NULL,
    [IDNumber] nvarchar(max) NULL,
    [IMEINO] nvarchar(max) NULL,
    [ClaimType] int NULL,
    [DamagePart] nvarchar(max) NULL,
    [ReplacementCost] float NULL,
    [IncidentDate] nvarchar(max) NOT NULL,
    [ClaimDate] nvarchar(max) NOT NULL,
    [Abstract] nvarchar(max) NULL,
    [Processed] bit NOT NULL,
    [CreatedOn] datetime2 NULL,
    CONSTRAINT [PK_claimRequests] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PhoneInsuranceRequest] (
    [Id] nvarchar(450) NOT NULL,
    [PartnerID] nvarchar(100) NULL,
    [ProductID] nvarchar(100) NULL,
    [CustomerName] nvarchar(200) NULL,
    [DateofBirth] nvarchar(200) NULL,
    [PhoneModel] nvarchar(200) NULL,
    [RequestId] nvarchar(200) NULL,
    [IMEINumber] nvarchar(200) NULL,
    [PhoneCost] float NULL,
    [ModeOfPurchase] int NULL,
    [LoanRefNumber] nvarchar(200) NULL,
    [RepaymentTerms] nvarchar(200) NULL,
    [LoanAmount] float NULL,
    [InterestRate] float NULL,
    [PremiumPaid] float NULL,
    [PurchaseDate] nvarchar(200) NULL,
    [Processed] bit NULL,
    [RequestedOn] datetime2 NULL,
    [PolicyStatus] int NULL,
    CONSTRAINT [PK_PhoneInsuranceRequest] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240129174401_claims', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240130071735_refno', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[claimRequests]') AND [c].[name] = N'IDNumber');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [claimRequests] DROP CONSTRAINT [' + @var0 + '];');
UPDATE [claimRequests] SET [IDNumber] = N'' WHERE [IDNumber] IS NULL;
ALTER TABLE [claimRequests] ALTER COLUMN [IDNumber] nvarchar(20) NOT NULL;
ALTER TABLE [claimRequests] ADD DEFAULT N'' FOR [IDNumber];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[claimRequests]') AND [c].[name] = N'CustomerName');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [claimRequests] DROP CONSTRAINT [' + @var1 + '];');
UPDATE [claimRequests] SET [CustomerName] = N'' WHERE [CustomerName] IS NULL;
ALTER TABLE [claimRequests] ALTER COLUMN [CustomerName] nvarchar(100) NOT NULL;
ALTER TABLE [claimRequests] ADD DEFAULT N'' FOR [CustomerName];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[claimRequests]') AND [c].[name] = N'ClaimRefNumber');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [claimRequests] DROP CONSTRAINT [' + @var2 + '];');
UPDATE [claimRequests] SET [ClaimRefNumber] = N'' WHERE [ClaimRefNumber] IS NULL;
ALTER TABLE [claimRequests] ALTER COLUMN [ClaimRefNumber] nvarchar(50) NOT NULL;
ALTER TABLE [claimRequests] ADD DEFAULT N'' FOR [ClaimRefNumber];
GO

ALTER TABLE [claimRequests] ADD [PartnerCode] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [claimRequests] ADD [ResponseId] nvarchar(50) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240130071850_refno1', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PhoneInsuranceRequest]') AND [c].[name] = N'CustomerName');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [PhoneInsuranceRequest] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [PhoneInsuranceRequest] DROP COLUMN [CustomerName];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PhoneInsuranceRequest]') AND [c].[name] = N'DateofBirth');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [PhoneInsuranceRequest] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [PhoneInsuranceRequest] DROP COLUMN [DateofBirth];
GO

EXEC sp_rename N'[PhoneInsuranceRequest].[ResponseId]', N'RequestId', N'COLUMN';
GO

EXEC sp_rename N'[claimRequests].[ResponseId]', N'RequestId', N'COLUMN';
GO

ALTER TABLE [PhoneInsuranceRequest] ADD [PhoneInsuranceCustomerId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [claimRequests] ADD [PhoneInsuranceCustomerId] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [phoneInsuranceCustomers] (
    [Id] int NOT NULL IDENTITY,
    [CustomerName] nvarchar(200) NULL,
    [PhoneNumber] nvarchar(200) NOT NULL,
    [IdNumber] nvarchar(200) NOT NULL,
    [CustomerAddress] nvarchar(200) NULL,
    [Nextofkinname] nvarchar(max) NULL,
    [NextofkinId] nvarchar(max) NULL,
    CONSTRAINT [PK_phoneInsuranceCustomers] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_PhoneInsuranceRequest_PhoneInsuranceCustomerId] ON [PhoneInsuranceRequest] ([PhoneInsuranceCustomerId]);
GO

CREATE INDEX [IX_claimRequests_PhoneInsuranceCustomerId] ON [claimRequests] ([PhoneInsuranceCustomerId]);
GO

ALTER TABLE [claimRequests] ADD CONSTRAINT [FK_claimRequests_phoneInsuranceCustomers_PhoneInsuranceCustomerId] FOREIGN KEY ([PhoneInsuranceCustomerId]) REFERENCES [phoneInsuranceCustomers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [PhoneInsuranceRequest] ADD CONSTRAINT [FK_PhoneInsuranceRequest_phoneInsuranceCustomers_PhoneInsuranceCustomerId] FOREIGN KEY ([PhoneInsuranceCustomerId]) REFERENCES [phoneInsuranceCustomers] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240206121755_phonecustomers', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [claimRequests] ADD [claimStatus] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240208063416_claimstatus', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [PartCosts] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Cost] float NOT NULL,
    [LabourCost] float NOT NULL,
    [Active] bit NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_PartCosts] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240305111059_parts', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartCosts]') AND [c].[name] = N'LabourCost');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [PartCosts] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [PartCosts] DROP COLUMN [LabourCost];
GO

CREATE TABLE [LabourCost] (
    [Id] int NOT NULL IDENTITY,
    [Desc] nvarchar(100) NULL,
    [Cost] float NOT NULL,
    [CreatedBy] nvarchar(100) NULL,
    [CreatedName] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [UpdatedName] nvarchar(100) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NULL,
    [DeletedOn] datetime2 NULL,
    [DeletedById] nvarchar(100) NULL,
    [DeletedByUser] nvarchar(100) NULL,
    CONSTRAINT [PK_LabourCost] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [RepairShops] (
    [Id] int NOT NULL IDENTITY,
    [ShopName] nvarchar(100) NOT NULL,
    [Phonenumber] nvarchar(100) NOT NULL,
    [ShopLocation] nvarchar(100) NOT NULL,
    [County] nvarchar(100) NULL,
    [Subcountry] nvarchar(100) NULL,
    [Ward] nvarchar(100) NULL,
    [Town] nvarchar(100) NULL,
    [Address] nvarchar(100) NULL,
    [SecondaryPhone] nvarchar(100) NULL,
    [Email] nvarchar(100) NULL,
    [ContactName] nvarchar(100) NULL,
    [ShopOwner] nvarchar(100) NULL,
    [iSActive] bit NOT NULL,
    [CreatedBy] nvarchar(100) NULL,
    [CreatedName] nvarchar(100) NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [UpdatedName] nvarchar(100) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NULL,
    [DeletedOn] datetime2 NULL,
    [DeletedById] nvarchar(100) NULL,
    [DeletedByUser] nvarchar(100) NULL,
    CONSTRAINT [PK_RepairShops] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240306091245_labortask', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [LabourCost];
GO

ALTER TABLE [RepairShops] ADD [loginType] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [RepairShops] ADD [shopType] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [PartCosts] ADD [PartCosts] float NOT NULL DEFAULT 0.0E0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240306133727_repairshopm', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ClaimPortalUser] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NULL,
    [Configuration] nvarchar(50) NULL,
    [IsEnabled] bit NOT NULL,
    [IpAddress] nvarchar(50) NULL,
    [HostUrl] nvarchar(50) NULL,
    [HostPort] nvarchar(50) NULL,
    [CreatedBy] nvarchar(50) NULL,
    [UpdatedBy] nvarchar(50) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [UserName] nvarchar(max) NULL,
    [NormalizedUserName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [NormalizedEmail] nvarchar(max) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_ClaimPortalUser] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240306135208_claimuers', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [ClaimPortalUser];
GO

EXEC sp_rename N'[PartCosts].[Cost]', N'LaborCost', N'COLUMN';
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RepairShops]') AND [c].[name] = N'ShopLocation');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [RepairShops] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [RepairShops] ALTER COLUMN [ShopLocation] nvarchar(max) NULL;
GO

ALTER TABLE [PartCosts] ADD [ReplacementLimit] int NOT NULL DEFAULT 0;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[claimRequests]') AND [c].[name] = N'IncidentDate');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [claimRequests] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [claimRequests] ALTER COLUMN [IncidentDate] datetime2 NOT NULL;
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[claimRequests]') AND [c].[name] = N'ClaimDate');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [claimRequests] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [claimRequests] ALTER COLUMN [ClaimDate] datetime2 NOT NULL;
GO

ALTER TABLE [claimRequests] ADD [PartId] int NULL;
GO

CREATE TABLE [PartnerAdminUser] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(50) NOT NULL,
    [FirstName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NULL,
    [Configuration] nvarchar(50) NULL,
    [IsEnabled] bit NOT NULL,
    [IpAddress] nvarchar(50) NULL,
    [HostUrl] nvarchar(50) NULL,
    [HostPort] nvarchar(50) NULL,
    [CreatedBy] nvarchar(50) NULL,
    [UpdatedBy] nvarchar(50) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [loginType] int NOT NULL,
    [UserName] nvarchar(50) NULL,
    [NormalizedUserName] nvarchar(50) NULL,
    [Email] nvarchar(50) NULL,
    [NormalizedEmail] nvarchar(50) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(50) NULL,
    [SecurityStamp] nvarchar(50) NULL,
    [ConcurrencyStamp] nvarchar(50) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_PartnerAdminUser] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240307075420_adminuser', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[claimRequests].[IMEINO]', N'Narration', N'COLUMN';
GO

ALTER TABLE [claimRequests] ADD [IMEINumber1] nvarchar(max) NULL;
GO

ALTER TABLE [claimRequests] ADD [IMEINumber2] nvarchar(max) NULL;
GO

ALTER TABLE [claimRequests] ADD [LabourCost] float NOT NULL DEFAULT 0.0E0;
GO

ALTER TABLE [claimRequests] ADD [PartCost] float NOT NULL DEFAULT 0.0E0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240307190348_claims1', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [claimRequests] ADD [Comments] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240311122726_claims12', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [claimRequests] ADD [PhoneId] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240311123440_PhoneId', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [PhoneInsuranceRequest] ADD [Active] bit NULL;
GO

ALTER TABLE [PhoneInsuranceRequest] ADD [IMEINumber1] nvarchar(max) NULL;
GO

ALTER TABLE [PhoneInsuranceRequest] ADD [IMEINumber2] nvarchar(max) NULL;
GO

ALTER TABLE [PhoneInsuranceRequest] ADD [PhoneName] nvarchar(200) NULL;
GO

ALTER TABLE [claimRequests] ADD [Dispatched] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [Message] nvarchar(max) NOT NULL,
    [notificationType] int NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240321144436_notifications', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [PortalActions] (
    [Id] bigint NOT NULL IDENTITY,
    [ActionName] nvarchar(max) NULL,
    [ActionDescription] nvarchar(max) NULL,
    [Shopname] nvarchar(max) NULL,
    [ShopLocation] nvarchar(max) NULL,
    [ShopName] nvarchar(max) NULL,
    [ShopType] nvarchar(max) NULL,
    [ClaimType] nvarchar(max) NULL,
    [IncidenceDate] nvarchar(max) NULL,
    [CustomerName] nvarchar(max) NULL,
    [CustomerIdNumber] nvarchar(max) NULL,
    [PhoneModel] nvarchar(max) NULL,
    [Reference] nvarchar(max) NULL,
    [RequestId] nvarchar(max) NULL,
    [CreatedOn] datetime2 NOT NULL,
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
    CONSTRAINT [PK_PortalActions] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240325041833_PortalActions', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [APIUSERS] DROP CONSTRAINT [FK_APIUSERS_Partners_PartnerId];
GO

ALTER TABLE [MsureRequests] DROP CONSTRAINT [FK_MsureRequests_Partners_PartnersId];
GO

ALTER TABLE [MsureRequests] DROP CONSTRAINT [FK_MsureRequests_partnersProducts_ProductsId];
GO

ALTER TABLE [partnersProducts] DROP CONSTRAINT [FK_partnersProducts_Partners_PartnerId];
GO

DROP TABLE [AspNetRoleClaims];
GO

DROP TABLE [AspNetUserClaims];
GO

DROP TABLE [AspNetUserLogins];
GO

DROP TABLE [AspNetUserRoles];
GO

DROP TABLE [AspNetUserTokens];
GO

DROP TABLE [claimRequests];
GO

DROP TABLE [Notifications];
GO

DROP TABLE [OnboardingRequests];
GO

DROP TABLE [PartCosts];
GO

DROP TABLE [PartnerAdminUser];
GO

DROP TABLE [PhoneInsuranceRequest];
GO

DROP TABLE [PortalActions];
GO

DROP TABLE [RepairShops];
GO

DROP TABLE [AspNetRoles];
GO

DROP TABLE [phoneInsuranceCustomers];
GO

ALTER TABLE [partnersProducts] DROP CONSTRAINT [PK_partnersProducts];
GO

DROP INDEX [IX_partnersProducts_PartnerId] ON [partnersProducts];
GO

ALTER TABLE [MsureRequests] DROP CONSTRAINT [PK_MsureRequests];
GO

DROP INDEX [EmailIndex] ON [APIUSERS];
GO

DROP INDEX [IX_APIUSERS_PartnerId] ON [APIUSERS];
GO

DROP INDEX [UserNameIndex] ON [APIUSERS];
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[partnersProducts]') AND [c].[name] = N'Description');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [partnersProducts] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [partnersProducts] DROP COLUMN [Description];
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'Email');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [Email];
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'EmailConfirmed');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [EmailConfirmed];
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'FirstName');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [FirstName];
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'LastName');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [LastName];
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'LockoutEnabled');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [LockoutEnabled];
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'LockoutEnd');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [LockoutEnd];
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'NormalizedEmail');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [NormalizedEmail];
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'NormalizedUserName');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [NormalizedUserName];
GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'PhoneNumberConfirmed');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [PhoneNumberConfirmed];
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'TwoFactorEnabled');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [TwoFactorEnabled];
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'UserName');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [APIUSERS] DROP COLUMN [UserName];
GO

EXEC sp_rename N'[partnersProducts]', N'PartnersProducts';
GO

EXEC sp_rename N'[MsureRequests]', N'msureRequests';
GO

EXEC sp_rename N'[msureRequests].[IX_MsureRequests_ProductsId]', N'IX_msureRequests_ProductsId', N'INDEX';
GO

EXEC sp_rename N'[msureRequests].[IX_MsureRequests_PartnersId]', N'IX_msureRequests_PartnersId', N'INDEX';
GO

EXEC sp_rename N'[APIUSERS].[AccessFailedCount]', N'accountType', N'COLUMN';
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PartnersProducts]') AND [c].[name] = N'PartnerId');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [PartnersProducts] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [PartnersProducts] ALTER COLUMN [PartnerId] nvarchar(max) NULL;
GO

ALTER TABLE [PartnersProducts] ADD [PartnerId1] int NOT NULL DEFAULT 0;
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'PartnerId');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [APIUSERS] ALTER COLUMN [PartnerId] nvarchar(max) NULL;
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'IsEnabled');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [APIUSERS] ALTER COLUMN [IsEnabled] bit NULL;
GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'ConsumerKey');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [APIUSERS] ALTER COLUMN [ConsumerKey] nvarchar(1000) NOT NULL;
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[APIUSERS]') AND [c].[name] = N'Configuration');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [APIUSERS] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [APIUSERS] ALTER COLUMN [Configuration] nvarchar(max) NULL;
GO

ALTER TABLE [PartnersProducts] ADD CONSTRAINT [PK_PartnersProducts] PRIMARY KEY ([Id]);
GO

ALTER TABLE [msureRequests] ADD CONSTRAINT [PK_msureRequests] PRIMARY KEY ([Id]);
GO

CREATE TABLE [Customers] (
    [Id] nvarchar(450) NOT NULL,
    [PartnerId] nvarchar(max) NOT NULL,
    [ProductId] int NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [DateOfBirth] nvarchar(max) NULL,
    [IDNumber] nvarchar(max) NULL,
    [Gender] int NULL,
    [Email] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Occupation] nvarchar(max) NULL,
    [Residency] nvarchar(max) NULL,
    [Nationality] nvarchar(max) NULL,
    [RequestDate] nvarchar(max) NULL,
    [Processed] bit NULL,
    [CreatedOn] datetime2 NOT NULL,
    [Status] nvarchar(max) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_PartnersProducts_PartnerId1] ON [PartnersProducts] ([PartnerId1]);
GO

ALTER TABLE [msureRequests] ADD CONSTRAINT [FK_msureRequests_PartnersProducts_ProductsId] FOREIGN KEY ([ProductsId]) REFERENCES [PartnersProducts] ([Id]);
GO

ALTER TABLE [msureRequests] ADD CONSTRAINT [FK_msureRequests_Partners_PartnersId] FOREIGN KEY ([PartnersId]) REFERENCES [Partners] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [PartnersProducts] ADD CONSTRAINT [FK_PartnersProducts_Partners_PartnerId1] FOREIGN KEY ([PartnerId1]) REFERENCES [Partners] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251223063829_InitialCreate', N'7.0.3');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'Processed');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var26 + '];');
UPDATE [Customers] SET [Processed] = CAST(0 AS bit) WHERE [Processed] IS NULL;
ALTER TABLE [Customers] ALTER COLUMN [Processed] bit NOT NULL;
ALTER TABLE [Customers] ADD DEFAULT CAST(0 AS bit) FOR [Processed];
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'CustomerName');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [Customers] ALTER COLUMN [CustomerName] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [AdditionalSourceOfIncome] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [AverageIncomeId] int NULL;
GO

ALTER TABLE [Customers] ADD [BusinessName] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [Complete] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Customers] ADD [EmployerName] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [EmploymentTerms] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [IdType] int NULL;
GO

ALTER TABLE [Customers] ADD [IsGroupMember] bit NULL;
GO

ALTER TABLE [Customers] ADD [LastModified] datetime2 NULL;
GO

ALTER TABLE [Customers] ADD [MailSent] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Customers] ADD [MailSentOn] datetime2 NULL;
GO

ALTER TABLE [Customers] ADD [Modified] bit NULL;
GO

ALTER TABLE [Customers] ADD [NatureOfBusiness] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [OtherNames] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [Pin] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [PolicyPath] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [RecordProcessed] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Customers] ADD [RetryCount] int NULL;
GO

ALTER TABLE [Customers] ADD [Role] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [RoleInBusiness] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [Signature] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [SignaturePath] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [SourceOfIncome] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [Stage] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Customers] ADD [Step] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Customers] ADD [Surname] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [TaxIdNumber] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [USAddress] nvarchar(50) NULL;
GO

ALTER TABLE [Customers] ADD [Validated] bit NULL;
GO

ALTER TABLE [Customers] ADD [ValidationDate] datetime2 NULL;
GO

ALTER TABLE [Customers] ADD [ValidationErrorCount] int NULL;
GO

ALTER TABLE [Customers] ADD [ValidationErrors] nvarchar(50) NULL;
GO

CREATE TABLE [CustomerHealthDeclarations] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] nvarchar(64) NOT NULL,
    [ContextKey] nvarchar(100) NULL,
    [HeightCm] decimal(9,2) NOT NULL,
    [WeightKg] decimal(9,2) NOT NULL,
    [Bmi] decimal(9,2) NOT NULL,
    [HealthDeclaration] bit NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedOn] datetime2 NULL,
    CONSTRAINT [PK_CustomerHealthDeclarations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Guardian] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] uniqueidentifier NOT NULL,
    [BeneficiaryId] bigint NOT NULL,
    [FirstName] nvarchar(50) NULL,
    [Surname] nvarchar(50) NULL,
    [OtherNames] nvarchar(50) NULL,
    [Email] nvarchar(50) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [Dob] nvarchar(50) NULL,
    [Gender] int NULL,
    [IdNumber] nvarchar(50) NULL,
    [IdType] nvarchar(50) NULL,
    [Relationship] nvarchar(50) NULL,
    [Status] nvarchar(50) NULL,
    [PartnerResponseDesc] nvarchar(50) NULL,
    [PartnerResponseCode] nvarchar(50) NULL,
    [Approved] bit NULL,
    [CreatedBy] nvarchar(50) NULL,
    [CreatedName] nvarchar(50) NULL,
    [UpdatedBy] nvarchar(50) NULL,
    [UpdatedName] nvarchar(50) NULL,
    [CreatedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [IsDeleted] bit NULL,
    [DeletedOn] datetime2 NULL,
    [DeletedById] nvarchar(50) NULL,
    [DeletedByUser] nvarchar(50) NULL,
    CONSTRAINT [PK_Guardian] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [CustomerHealthDeclarationAnswers] (
    [Id] uniqueidentifier NOT NULL,
    [DeclarationId] uniqueidentifier NOT NULL,
    [Question] int NOT NULL,
    [Answer] bit NOT NULL,
    [Narration] nvarchar(Max) NULL,
    [CreatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_CustomerHealthDeclarationAnswers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CustomerHealthDeclarationAnswers_CustomerHealthDeclarations_DeclarationId] FOREIGN KEY ([DeclarationId]) REFERENCES [CustomerHealthDeclarations] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Beneficiaries] (
    [Id] bigint NOT NULL IDENTITY,
    [CustomerId] nvarchar(450) NOT NULL,
    [PartnerBeneficiaryCode] nvarchar(50) NULL,
    [Firstname] nvarchar(50) NULL,
    [OtherNames] nvarchar(50) NULL,
    [EmailAddress] nvarchar(50) NULL,
    [Gender] int NULL,
    [Phone] nvarchar(50) NULL,
    [Percentage] decimal(18,2) NULL,
    [Relationship] nvarchar(50) NULL,
    [IdNumber] nvarchar(50) NULL,
    [Date_of_birthRaw] nvarchar(50) NULL,
    [Date_of_birth] nvarchar(50) NULL,
    [Age] int NULL,
    [Status] nvarchar(50) NULL,
    [GuardianId] bigint NULL,
    [PartnerResponseDesc] nvarchar(50) NULL,
    [PartnerResponseCode] nvarchar(50) NULL,
    [Approved] nvarchar(50) NULL,
    [Confirmed] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [IsLocked] bit NOT NULL,
    [Surname] nvarchar(50) NULL,
    [CreatedBy] nvarchar(50) NULL,
    [CreatedName] nvarchar(50) NULL,
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

CREATE INDEX [IX_Beneficiaries_GuardianId] ON [Beneficiaries] ([GuardianId]);
GO

CREATE INDEX [IX_CustomerHealthDeclarationAnswers_DeclarationId] ON [CustomerHealthDeclarationAnswers] ([DeclarationId]);
GO

CREATE UNIQUE INDEX [IX_CustomerHealthDeclarationAnswers_DeclarationId_Question] ON [CustomerHealthDeclarationAnswers] ([DeclarationId], [Question]);
GO

CREATE UNIQUE INDEX [IX_CustomerHealthDeclarations_CustomerId_ContextKey] ON [CustomerHealthDeclarations] ([CustomerId], [ContextKey]) WHERE [ContextKey] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260213155539_beneficiary', N'7.0.3');
GO

COMMIT;
GO

