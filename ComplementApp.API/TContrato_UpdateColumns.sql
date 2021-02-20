BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210219171349_TContrato_UpdateColumns')
BEGIN
    DECLARE @var50 sysname;
    SELECT @var50 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TContrato]') AND [c].[name] = N'Supervisor1Id');
    IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [TContrato] DROP CONSTRAINT [' + @var50 + '];');
    ALTER TABLE [TContrato] ALTER COLUMN [Supervisor1Id] int NOT NULL;
    ALTER TABLE [TContrato] ADD DEFAULT 0 FOR [Supervisor1Id];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210219171349_TContrato_UpdateColumns')
BEGIN
    DECLARE @var51 sysname;
    SELECT @var51 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TContrato]') AND [c].[name] = N'FechaExpedicionPoliza');
    IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [TContrato] DROP CONSTRAINT [' + @var51 + '];');
    ALTER TABLE [TContrato] ALTER COLUMN [FechaExpedicionPoliza] datetime2 NOT NULL;
    ALTER TABLE [TContrato] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [FechaExpedicionPoliza];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210219171349_TContrato_UpdateColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210219171349_TContrato_UpdateColumns', N'5.0.0');
END;
GO

COMMIT;
GO

