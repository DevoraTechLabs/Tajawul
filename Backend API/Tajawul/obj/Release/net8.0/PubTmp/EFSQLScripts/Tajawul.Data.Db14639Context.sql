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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [languages] (
        [language_id] int NOT NULL IDENTITY,
        [language_name] nvarchar(255) NOT NULL,
        [language_code] nvarchar(50) NOT NULL,
        [created_at] datetime NOT NULL DEFAULT ((getdate())),
        [updated_at] datetime NOT NULL DEFAULT ((getdate())),
        CONSTRAINT [PK__language__804CF6B33AE2B7B9] PRIMARY KEY ([language_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [persons] (
        [Id] nvarchar(450) NOT NULL,
        [first_name] nvarchar(100) NOT NULL,
        [last_name] nvarchar(100) NOT NULL,
        [profile_picture_URL] nvarchar(max) NULL,
        [birth_date] date NULL,
        [last_login] datetime NULL,
        [created_at] datetime NOT NULL DEFAULT ((getdate())),
        [updated_at] datetime NOT NULL DEFAULT ((getdate())),
        [postal_code] nvarchar(20) NULL,
        [city] nvarchar(100) NULL,
        [country] nvarchar(100) NULL,
        [street] nvarchar(255) NULL,
        [location] nvarchar(max) NULL,
        [UserName] nvarchar(100) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [email] nvarchar(255) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [email_confirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [phone_number] nvarchar(30) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK__persons__543848DFB153A447] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_persons_UserId] FOREIGN KEY ([UserId]) REFERENCES [persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_persons_UserId] FOREIGN KEY ([UserId]) REFERENCES [persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_persons_UserId] FOREIGN KEY ([UserId]) REFERENCES [persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_persons_UserId] FOREIGN KEY ([UserId]) REFERENCES [persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [business_managers] (
        [manager_id] nvarchar(450) NOT NULL,
        [Id] nvarchar(450) NOT NULL,
        CONSTRAINT [PK__business__5A6073FC19907B65] PRIMARY KEY ([manager_id]),
        CONSTRAINT [FK__business___perso__5BE2A6F2] FOREIGN KEY ([Id]) REFERENCES [persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [place_owners] (
        [owner_id] nvarchar(450) NOT NULL,
        [Id] nvarchar(450) NOT NULL,
        [national_number] nvarchar(50) NOT NULL,
        [document_image_URL] nvarchar(max) NULL,
        CONSTRAINT [PK__place_ow__3C4FBEE4397EA56B] PRIMARY KEY ([owner_id]),
        CONSTRAINT [FK__place_own__perso__59063A47] FOREIGN KEY ([Id]) REFERENCES [persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE TABLE [users] (
        [user_id] nvarchar(450) NOT NULL,
        [Id] nvarchar(450) NOT NULL,
        [bio] nvarchar(max) NULL,
        [gender] bit NULL,
        [is_top_traveler] bit NOT NULL,
        [completed_survey] bit NOT NULL,
        [social_media_links] nvarchar(max) NULL,
        CONSTRAINT [PK__users__B9BE370FA5584F82] PRIMARY KEY ([user_id]),
        CONSTRAINT [FK__users__person_id__5165187F] FOREIGN KEY ([Id]) REFERENCES [persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [IX_business_managers_Id] ON [business_managers] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [persons] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UQ__persons__7C9273C45FD361E4] ON [persons] ([UserName]) WHERE [UserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UQ__persons__AB6E616487425769] ON [persons] ([email]) WHERE [email] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [persons] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [IX_place_owners_Id] ON [place_owners] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE UNIQUE INDEX [UQ__place_ow__4449273AD0EF3137] ON [place_owners] ([national_number]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    CREATE INDEX [IX_users_Id] ON [users] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250131194625_Identity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250131194625_Identity', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250201181440_AddRefreshTokenColumn'
)
BEGIN
    ALTER TABLE [Persons] ADD [RefreshToken] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250201181440_AddRefreshTokenColumn'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250201181440_AddRefreshTokenColumn', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'bio');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [users] DROP COLUMN [bio];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'completed_survey');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [users] DROP COLUMN [completed_survey];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'gender');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [users] DROP COLUMN [gender];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'is_top_traveler');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [users] DROP COLUMN [is_top_traveler];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[users]') AND [c].[name] = N'social_media_links');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [users] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [users] DROP COLUMN [social_media_links];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'birth_date');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [persons] DROP COLUMN [birth_date];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'city');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [persons] DROP COLUMN [city];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'country');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [persons] DROP COLUMN [country];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'location');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [persons] DROP COLUMN [location];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'postal_code');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [persons] DROP COLUMN [postal_code];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'profile_picture_URL');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [persons] DROP COLUMN [profile_picture_URL];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'street');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [persons] DROP COLUMN [street];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    EXEC sp_rename N'[persons].[phone_number]', N'PhoneNumber', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'PhoneNumber');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [persons] ALTER COLUMN [PhoneNumber] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227163954_removeFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250227163954_removeFields', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227185719_seedRole'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''251467ac-f694-48c3-922a-46b27ffbbe03'', NULL, N''PlaceOwner'', N''PLACEOWNER''),
    (N''704a2a66-bc99-4bbe-9ebc-5b74f5df1362'', NULL, N''User'', N''USER''),
    (N''897cb339-f935-4616-ac91-204fbdb49a03'', NULL, N''BusinessManager'', N''BUSINESSMANAGER''),
    (N''9c1358a4-5151-4888-ba77-5a71174fabd4'', NULL, N''Person'', N''PERSON'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227185719_seedRole'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250227185719_seedRole', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227234028_remove_user_businessManager_tables'
)
BEGIN
    DROP TABLE [business_managers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227234028_remove_user_businessManager_tables'
)
BEGIN
    DROP TABLE [users];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250227234028_remove_user_businessManager_tables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250227234028_remove_user_businessManager_tables', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250303021133_addRole'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''8c1683a7-f52e-4f82-9b88-513ab45fc1c8'', NULL, N''CompletedStepOne'', N''COMPLETEDSTEPONE'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250303021133_addRole'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250303021133_addRole', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250304153216_updateRoles'
)
BEGIN
    EXEC(N'UPDATE [AspNetRoles] SET [Name] = N''CompletedSocialInfo'', [NormalizedName] = N''COMPLETEDSOCIALINFO''
    WHERE [Id] = N''8c1683a7-f52e-4f82-9b88-513ab45fc1c8'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250304153216_updateRoles'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''1e84b668-1a0f-4040-a602-3949f371793e'', NULL, N''CompletedInterestInfo'', N''COMPLETEDINTERESTINFO'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250304153216_updateRoles'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250304153216_updateRoles', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307014157_removeFirstAndLastName'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'first_name');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [persons] DROP COLUMN [first_name];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307014157_removeFirstAndLastName'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'last_name');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [persons] DROP COLUMN [last_name];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307014157_removeFirstAndLastName'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250307014157_removeFirstAndLastName', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307015440_removeUsername'
)
BEGIN
    DROP INDEX [UQ__persons__7C9273C45FD361E4] ON [persons];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307015440_removeUsername'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'UserName');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [persons] ALTER COLUMN [UserName] nvarchar(256) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307015440_removeUsername'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250307015440_removeUsername', N'9.0.1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307020734_addUsername'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[persons]') AND [c].[name] = N'UserName');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [persons] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [persons] ALTER COLUMN [UserName] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307020734_addUsername'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UQ__persons__7C9273C45FD361E4] ON [persons] ([UserName]) WHERE [UserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250307020734_addUsername'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250307020734_addUsername', N'9.0.1');
END;

COMMIT;
GO

