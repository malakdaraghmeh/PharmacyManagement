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

CREATE TABLE [Users] (
    [Id] nvarchar(450) NOT NULL,
    [Username] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [PharmacyName] nvarchar(200) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [CreditRecords] (
    [Id] nvarchar(450) NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerPhone] nvarchar(max) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [PaidAmount] decimal(18,2) NOT NULL,
    [RemainingAmount] decimal(18,2) NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_CreditRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CreditRecords_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Drugs] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Barcode] nvarchar(50) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [CostPrice] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    [MinimumStock] int NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [Manufacturer] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Drugs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Drugs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Notifications] (
    [Id] nvarchar(450) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Sales] (
    [Id] nvarchar(450) NOT NULL,
    [InvoiceNumber] nvarchar(450) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [DiscountAmount] decimal(18,2) NOT NULL,
    [NetAmount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerPhone] nvarchar(max) NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Sales] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sales_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [SaleItems] (
    [Id] nvarchar(450) NOT NULL,
    [SaleId] nvarchar(450) NOT NULL,
    [DrugId] nvarchar(450) NOT NULL,
    [DrugName] nvarchar(max) NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [DiscountPercentage] decimal(5,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SaleItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SaleItems_Drugs_DrugId] FOREIGN KEY ([DrugId]) REFERENCES [Drugs] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SaleItems_Sales_SaleId] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CreditRecords_UserId] ON [CreditRecords] ([UserId]);
GO

CREATE INDEX [IX_Drugs_Barcode] ON [Drugs] ([Barcode]);
GO

CREATE INDEX [IX_Drugs_UserId] ON [Drugs] ([UserId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_SaleItems_DrugId] ON [SaleItems] ([DrugId]);
GO

CREATE INDEX [IX_SaleItems_SaleId] ON [SaleItems] ([SaleId]);
GO

CREATE UNIQUE INDEX [IX_Sales_InvoiceNumber] ON [Sales] ([InvoiceNumber]);
GO

CREATE INDEX [IX_Sales_UserId] ON [Sales] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260125200952_InitialCreate', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_Users_Username] ON [Users];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Username');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Users] DROP COLUMN [Username];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SaleItems]') AND [c].[name] = N'DiscountPercentage');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [SaleItems] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [SaleItems] DROP COLUMN [DiscountPercentage];
GO

EXEC sp_rename N'[Users].[Address]', N'City', N'COLUMN';
GO

ALTER TABLE [Drugs] ADD [BatchNumber] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260128221635_UpdateUserDrugSaleFields', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CreditRecords]') AND [c].[name] = N'Status');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [CreditRecords] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [CreditRecords] ALTER COLUMN [Status] int NOT NULL;
GO

ALTER TABLE [CreditRecords] ADD [Type] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260216203257_UpdateCreditRecordEnum', N'8.0.23');
GO

COMMIT;
GO

