
BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210213124502_TTercero_AddAuditory')
BEGIN
    ALTER TABLE [TTercero] ADD [FechaModificacion] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210213124502_TTercero_AddAuditory')
BEGIN
    ALTER TABLE [TTercero] ADD [FechaRegistro] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210213124502_TTercero_AddAuditory')
BEGIN
    ALTER TABLE [TTercero] ADD [UsuarioIdModificacion] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210213124502_TTercero_AddAuditory')
BEGIN
    ALTER TABLE [TTercero] ADD [UsuarioIdRegistro] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210213124502_TTercero_AddAuditory')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210213124502_TTercero_AddAuditory', N'5.0.0');
END;
GO

COMMIT;
GO

