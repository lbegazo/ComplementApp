
BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210220232935_TParametrizacionLiquidacionTercero_NotaLegal6')
BEGIN
    ALTER TABLE [TParametroLiquidacionTercero] ADD [NotaLegal6] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210220232935_TParametrizacionLiquidacionTercero_NotaLegal6')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210220232935_TParametrizacionLiquidacionTercero_NotaLegal6', N'5.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210221104500_TNumeracion_Create')
BEGIN
    CREATE TABLE [TNumeracion] (
        [NumeracionId] int NOT NULL IDENTITY,
        [Consecutivo] int NOT NULL,
        [NumeroConsecutivo] VARCHAR(10) NOT NULL,
        [FormatoSolicitudPagoId] int NULL,
        [Utilizado] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_TNumeracion] PRIMARY KEY ([NumeracionId]),
        CONSTRAINT [FK_TNumeracion_TFormatoSolicitudPago_FormatoSolicitudPagoId] FOREIGN KEY ([FormatoSolicitudPagoId]) REFERENCES [TFormatoSolicitudPago] ([FormatoSolicitudPagoId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210221104500_TNumeracion_Create')
BEGIN
    CREATE INDEX [IX_TNumeracion_FormatoSolicitudPagoId] ON [TNumeracion] ([FormatoSolicitudPagoId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210221104500_TNumeracion_Create')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210221104500_TNumeracion_Create', N'5.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210222010147_TTercero_DropColumnFacturaElectronica')
BEGIN
    DECLARE @var52 sysname;
    SELECT @var52 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TTercero]') AND [c].[name] = N'FacturadorElectronico');
    IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [TTercero] DROP CONSTRAINT [' + @var52 + '];');
    ALTER TABLE [TTercero] DROP COLUMN [FacturadorElectronico];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210222010147_TTercero_DropColumnFacturaElectronica')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210222010147_TTercero_DropColumnFacturaElectronica', N'5.0.0');
END;
GO

COMMIT;
GO

