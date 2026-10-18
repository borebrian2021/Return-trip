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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AdminCommissionSettings] (
        [Id] int NOT NULL IDENTITY,
        [Percentage] decimal(18,2) NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AdminCommissionSettings] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [IsApproved] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
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
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [GoodsOwners] (
        [Id] int NOT NULL IDENTITY,
        [UserAccountId] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Location] nvarchar(max) NOT NULL,
        [IsVerified] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_GoodsOwners] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GoodsOwners_AspNetUsers_UserAccountId] FOREIGN KEY ([UserAccountId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [VehicleOwners] (
        [Id] int NOT NULL IDENTITY,
        [UserAccountId] int NOT NULL,
        [CompanyName] nvarchar(max) NOT NULL,
        [BusinessRegistrationNumber] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Location] nvarchar(max) NOT NULL,
        [IsApproved] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_VehicleOwners] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VehicleOwners_AspNetUsers_UserAccountId] FOREIGN KEY ([UserAccountId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [Cargoes] (
        [Id] int NOT NULL IDENTITY,
        [GoodsOwnerId] int NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [FromLocation] nvarchar(300) NOT NULL,
        [FromPlaceId] nvarchar(200) NULL,
        [FromLatitude] decimal(9,6) NULL,
        [FromLongitude] decimal(9,6) NULL,
        [ToLocation] nvarchar(300) NOT NULL,
        [ToPlaceId] nvarchar(200) NULL,
        [ToLatitude] decimal(9,6) NULL,
        [ToLongitude] decimal(9,6) NULL,
        [RequiredDepartureAt] datetime2 NULL,
        [DeliveryDeadlineAt] datetime2 NULL,
        [DepartedAt] datetime2 NULL,
        [DeliveredAt] datetime2 NULL,
        [RetentionExpiresAt] datetime2 NULL,
        [ReopenedAt] datetime2 NULL,
        [WeightKg] decimal(12,3) NOT NULL,
        [PackageType] nvarchar(max) NOT NULL,
        [PricingMode] nvarchar(max) NOT NULL,
        [PricePerKg] decimal(18,2) NULL,
        [TransportCost] decimal(18,2) NOT NULL,
        [CommissionPercent] decimal(5,2) NOT NULL,
        [RetentionPeriodDays] int NOT NULL,
        [Status] nvarchar(450) NOT NULL,
        [AdvancePaymentPercent] decimal(5,2) NOT NULL,
        [IsBidOpen] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Cargoes] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Cargo_ActualDates] CHECK ([DepartedAt] IS NULL OR [DeliveredAt] IS NULL OR [DeliveredAt] >= [DepartedAt]),
        CONSTRAINT [CK_Cargo_FromCoordinates] CHECK (([FromLatitude] IS NULL OR [FromLatitude] BETWEEN -90 AND 90) AND ([FromLongitude] IS NULL OR [FromLongitude] BETWEEN -180 AND 180)),
        CONSTRAINT [CK_Cargo_PricingMode] CHECK (([PricingMode] = 'FixedTotal' AND [PricePerKg] IS NULL) OR ([PricingMode] = 'PerKg' AND [WeightKg] > 0 AND [PricePerKg] > 0)),
        CONSTRAINT [CK_Cargo_RetentionExpiry] CHECK ([RetentionExpiresAt] IS NULL OR [RequiredDepartureAt] IS NULL OR [RetentionExpiresAt] >= [RequiredDepartureAt]),
        CONSTRAINT [CK_Cargo_ScheduledDates] CHECK ([RequiredDepartureAt] IS NULL OR [DeliveryDeadlineAt] IS NULL OR [DeliveryDeadlineAt] >= [RequiredDepartureAt]),
        CONSTRAINT [CK_Cargo_ToCoordinates] CHECK (([ToLatitude] IS NULL OR [ToLatitude] BETWEEN -90 AND 90) AND ([ToLongitude] IS NULL OR [ToLongitude] BETWEEN -180 AND 180)),
        CONSTRAINT [FK_Cargoes_GoodsOwners_GoodsOwnerId] FOREIGN KEY ([GoodsOwnerId]) REFERENCES [GoodsOwners] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [BankDetails] (
        [Id] int NOT NULL IDENTITY,
        [VehicleOwnerId] int NULL,
        [GoodsOwnerId] int NULL,
        [BankName] nvarchar(max) NOT NULL,
        [AccountName] nvarchar(max) NOT NULL,
        [AccountNumber] nvarchar(max) NOT NULL,
        [AccountType] nvarchar(max) NOT NULL,
        [SwiftCode] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_BankDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BankDetails_GoodsOwners_GoodsOwnerId] FOREIGN KEY ([GoodsOwnerId]) REFERENCES [GoodsOwners] ([Id]),
        CONSTRAINT [FK_BankDetails_VehicleOwners_VehicleOwnerId] FOREIGN KEY ([VehicleOwnerId]) REFERENCES [VehicleOwners] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [CompanyRegistrations] (
        [Id] int NOT NULL IDENTITY,
        [VehicleOwnerId] int NOT NULL,
        [CompanyName] nvarchar(max) NOT NULL,
        [RegistrationNumber] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Location] nvarchar(max) NOT NULL,
        [NumberPlate] nvarchar(max) NOT NULL,
        [CertificateOfRegistrationUrl] nvarchar(max) NOT NULL,
        [CertificateOfRegistrationType] nvarchar(max) NOT NULL,
        [IsApproved] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CompanyRegistrations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CompanyRegistrations_VehicleOwners_VehicleOwnerId] FOREIGN KEY ([VehicleOwnerId]) REFERENCES [VehicleOwners] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [Drivers] (
        [Id] int NOT NULL IDENTITY,
        [UserAccountId] int NULL,
        [VehicleOwnerId] int NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [IdNumber] nvarchar(450) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [LicenseNumber] nvarchar(max) NOT NULL,
        [LicenseImageUrl] nvarchar(max) NOT NULL,
        [LicenseImageType] nvarchar(max) NOT NULL,
        [IsVerified] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Drivers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Drivers_AspNetUsers_UserAccountId] FOREIGN KEY ([UserAccountId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Drivers_VehicleOwners_VehicleOwnerId] FOREIGN KEY ([VehicleOwnerId]) REFERENCES [VehicleOwners] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [UserAccountId] int NULL,
        [VehicleOwnerId] int NULL,
        [Title] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [RelatedEntityType] nvarchar(max) NULL,
        [RelatedEntityId] int NULL,
        [IsRead] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Notifications_ExactlyOneRecipient] CHECK ((([UserAccountId] IS NOT NULL AND [VehicleOwnerId] IS NULL) OR ([UserAccountId] IS NULL AND [VehicleOwnerId] IS NOT NULL))),
        CONSTRAINT [FK_Notifications_AspNetUsers_UserAccountId] FOREIGN KEY ([UserAccountId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notifications_VehicleOwners_VehicleOwnerId] FOREIGN KEY ([VehicleOwnerId]) REFERENCES [VehicleOwners] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [CompanyAgents] (
        [Id] int NOT NULL IDENTITY,
        [UserAccountId] int NOT NULL,
        [VehicleOwnerId] int NOT NULL,
        [CompanyId] int NULL,
        [FullName] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_CompanyAgents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CompanyAgents_AspNetUsers_UserAccountId] FOREIGN KEY ([UserAccountId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CompanyAgents_CompanyRegistrations_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [CompanyRegistrations] ([Id]),
        CONSTRAINT [FK_CompanyAgents_VehicleOwners_VehicleOwnerId] FOREIGN KEY ([VehicleOwnerId]) REFERENCES [VehicleOwners] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [VehicleDetails] (
        [Id] int NOT NULL IDENTITY,
        [VehicleOwnerId] int NOT NULL,
        [CompanyId] int NOT NULL,
        [VehicleType] nvarchar(max) NOT NULL,
        [RegistrationNumber] nvarchar(max) NOT NULL,
        [LicenseNumber] nvarchar(max) NOT NULL,
        [LicenseDocumentUrl] nvarchar(max) NOT NULL,
        [LicenseDocumentType] nvarchar(max) NOT NULL,
        [IsApproved] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_VehicleDetails] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VehicleDetails_CompanyRegistrations_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [CompanyRegistrations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_VehicleDetails_VehicleOwners_VehicleOwnerId] FOREIGN KEY ([VehicleOwnerId]) REFERENCES [VehicleOwners] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [VehicleRoutes] (
        [Id] int NOT NULL IDENTITY,
        [VehicleDetailId] int NOT NULL,
        [CompanyAgentId] int NOT NULL,
        [FromLocation] nvarchar(300) NOT NULL,
        [FromPlaceId] nvarchar(200) NULL,
        [FromLatitude] decimal(9,6) NULL,
        [FromLongitude] decimal(9,6) NULL,
        [ToLocation] nvarchar(300) NOT NULL,
        [ToPlaceId] nvarchar(200) NULL,
        [ToLatitude] decimal(9,6) NULL,
        [ToLongitude] decimal(9,6) NULL,
        [TripStartsAt] datetime2 NOT NULL,
        [ExpectedArrivalAt] datetime2 NOT NULL,
        [ReturnAvailabilityStartsAt] datetime2 NULL,
        [ReturnAvailabilityEndsAt] datetime2 NULL,
        [IsAwaitingReturn] bit NOT NULL,
        [RoutePathJson] nvarchar(max) NULL,
        [MapProvider] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_VehicleRoutes] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_VehicleRoutes_FromCoordinates] CHECK (([FromLatitude] IS NULL OR [FromLatitude] BETWEEN -90 AND 90) AND ([FromLongitude] IS NULL OR [FromLongitude] BETWEEN -180 AND 180)),
        CONSTRAINT [CK_VehicleRoutes_ReturnWindow] CHECK ([ReturnAvailabilityEndsAt] IS NULL OR [ReturnAvailabilityStartsAt] IS NULL OR [ReturnAvailabilityEndsAt] >= [ReturnAvailabilityStartsAt]),
        CONSTRAINT [CK_VehicleRoutes_ToCoordinates] CHECK (([ToLatitude] IS NULL OR [ToLatitude] BETWEEN -90 AND 90) AND ([ToLongitude] IS NULL OR [ToLongitude] BETWEEN -180 AND 180)),
        CONSTRAINT [CK_VehicleRoutes_TripTimes] CHECK ([ExpectedArrivalAt] >= [TripStartsAt]),
        CONSTRAINT [FK_VehicleRoutes_CompanyAgents_CompanyAgentId] FOREIGN KEY ([CompanyAgentId]) REFERENCES [CompanyAgents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_VehicleRoutes_VehicleDetails_VehicleDetailId] FOREIGN KEY ([VehicleDetailId]) REFERENCES [VehicleDetails] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [CargoBids] (
        [Id] int NOT NULL IDENTITY,
        [CargoId] int NOT NULL,
        [CompanyAgentId] int NOT NULL,
        [VehicleRouteId] int NOT NULL,
        [DriverId] int NULL,
        [BidAmount] decimal(18,2) NOT NULL,
        [SuggestedAmount] decimal(18,2) NULL,
        [SuggestionComments] nvarchar(max) NULL,
        [PriceSuggestionStatus] nvarchar(max) NOT NULL,
        [PriceDecisionAt] datetime2 NULL,
        [PriceDecidedByGoodsOwnerId] int NULL,
        [EstimatedVehicleArrivalAt] datetime2 NULL,
        [Status] nvarchar(450) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [AcceptedAt] datetime2 NULL,
        [CompletedAt] datetime2 NULL,
        CONSTRAINT [PK_CargoBids] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CargoBids_Cargoes_CargoId] FOREIGN KEY ([CargoId]) REFERENCES [Cargoes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CargoBids_CompanyAgents_CompanyAgentId] FOREIGN KEY ([CompanyAgentId]) REFERENCES [CompanyAgents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CargoBids_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_CargoBids_GoodsOwners_PriceDecidedByGoodsOwnerId] FOREIGN KEY ([PriceDecidedByGoodsOwnerId]) REFERENCES [GoodsOwners] ([Id]),
        CONSTRAINT [FK_CargoBids_VehicleRoutes_VehicleRouteId] FOREIGN KEY ([VehicleRouteId]) REFERENCES [VehicleRoutes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE TABLE [DeliveryReceipts] (
        [Id] int NOT NULL IDENTITY,
        [CargoBidId] int NOT NULL,
        [DriverId] int NOT NULL,
        [ReceiverName] nvarchar(max) NOT NULL,
        [SignedAt] datetime2 NOT NULL,
        [ImageUrl] nvarchar(max) NOT NULL,
        [ImageContentType] nvarchar(max) NOT NULL,
        [ImageFileName] nvarchar(max) NULL,
        [ConfirmationStatus] nvarchar(450) NOT NULL,
        [ConfirmedByCompanyAgentId] int NULL,
        [ConfirmedAt] datetime2 NULL,
        [AgentComments] nvarchar(max) NULL,
        [UploadedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_DeliveryReceipts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeliveryReceipts_CargoBids_CargoBidId] FOREIGN KEY ([CargoBidId]) REFERENCES [CargoBids] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DeliveryReceipts_CompanyAgents_ConfirmedByCompanyAgentId] FOREIGN KEY ([ConfirmedByCompanyAgentId]) REFERENCES [CompanyAgents] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_DeliveryReceipts_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]) WHERE [NormalizedEmail] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_BankDetails_GoodsOwnerId] ON [BankDetails] ([GoodsOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_BankDetails_VehicleOwnerId] ON [BankDetails] ([VehicleOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CargoBids_CargoId_CompanyAgentId] ON [CargoBids] ([CargoId], [CompanyAgentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CargoBids_CompanyAgentId] ON [CargoBids] ([CompanyAgentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CargoBids_DriverId] ON [CargoBids] ([DriverId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CargoBids_PriceDecidedByGoodsOwnerId] ON [CargoBids] ([PriceDecidedByGoodsOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CargoBids_Status_CreatedAt] ON [CargoBids] ([Status], [CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CargoBids_VehicleRouteId] ON [CargoBids] ([VehicleRouteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_Cargoes_GoodsOwnerId] ON [Cargoes] ([GoodsOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_Cargoes_Status_RetentionExpiresAt] ON [Cargoes] ([Status], [RetentionExpiresAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CompanyAgents_CompanyId] ON [CompanyAgents] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CompanyAgents_UserAccountId] ON [CompanyAgents] ([UserAccountId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CompanyAgents_VehicleOwnerId] ON [CompanyAgents] ([VehicleOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_CompanyRegistrations_VehicleOwnerId] ON [CompanyRegistrations] ([VehicleOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DeliveryReceipts_CargoBidId] ON [DeliveryReceipts] ([CargoBidId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_DeliveryReceipts_ConfirmationStatus] ON [DeliveryReceipts] ([ConfirmationStatus]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_DeliveryReceipts_ConfirmedByCompanyAgentId] ON [DeliveryReceipts] ([ConfirmedByCompanyAgentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_DeliveryReceipts_DriverId] ON [DeliveryReceipts] ([DriverId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Drivers_IdNumber] ON [Drivers] ([IdNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_Drivers_UserAccountId] ON [Drivers] ([UserAccountId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_Drivers_VehicleOwnerId] ON [Drivers] ([VehicleOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_GoodsOwners_UserAccountId] ON [GoodsOwners] ([UserAccountId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserAccountId_IsRead_CreatedAt] ON [Notifications] ([UserAccountId], [IsRead], [CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_Notifications_VehicleOwnerId] ON [Notifications] ([VehicleOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_VehicleDetails_CompanyId] ON [VehicleDetails] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_VehicleDetails_VehicleOwnerId] ON [VehicleDetails] ([VehicleOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_VehicleOwners_UserAccountId] ON [VehicleOwners] ([UserAccountId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_VehicleRoutes_CompanyAgentId] ON [VehicleRoutes] ([CompanyAgentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_VehicleRoutes_IsAwaitingReturn_ReturnAvailabilityStartsAt_ReturnAvailabilityEndsAt] ON [VehicleRoutes] ([IsAwaitingReturn], [ReturnAvailabilityStartsAt], [ReturnAvailabilityEndsAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    CREATE INDEX [IX_VehicleRoutes_VehicleDetailId] ON [VehicleRoutes] ([VehicleDetailId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081503_IdentityAndDriverClaim'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011081503_IdentityAndDriverClaim', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081829_AgentProfileClaim'
)
BEGIN
    ALTER TABLE [CompanyAgents] DROP CONSTRAINT [FK_CompanyAgents_AspNetUsers_UserAccountId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081829_AgentProfileClaim'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyAgents]') AND [c].[name] = N'UserAccountId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [CompanyAgents] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [CompanyAgents] ALTER COLUMN [UserAccountId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081829_AgentProfileClaim'
)
BEGIN
    ALTER TABLE [CompanyAgents] ADD CONSTRAINT [FK_CompanyAgents_AspNetUsers_UserAccountId] FOREIGN KEY ([UserAccountId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011081829_AgentProfileClaim'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011081829_AgentProfileClaim', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261012162617_Initial migration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261012162617_Initial migration', N'8.0.0');
END;
GO

COMMIT;
GO

