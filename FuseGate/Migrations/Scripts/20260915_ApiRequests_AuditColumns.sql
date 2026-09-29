-- Extend ApiRequests for middleware audit capture.
-- Run against the FuseGate / Application database.

IF COL_LENGTH('dbo.ApiRequests', 'PartnerCode') IS NULL
    ALTER TABLE [dbo].[ApiRequests] ADD [PartnerCode] nvarchar(50) NULL;

IF COL_LENGTH('dbo.ApiRequests', 'PartnerName') IS NULL
    ALTER TABLE [dbo].[ApiRequests] ADD [PartnerName] nvarchar(200) NULL;

IF COL_LENGTH('dbo.ApiRequests', 'ResponseCode') IS NULL
    ALTER TABLE [dbo].[ApiRequests] ADD [ResponseCode] int NULL;
