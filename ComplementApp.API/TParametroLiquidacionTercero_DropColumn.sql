
BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210224164150_TParametroLiquidacionTercero_DropColumnSupervisor')
BEGIN
    ALTER TABLE [TParametroLiquidacionTercero] DROP CONSTRAINT [FK_TParametroLiquidacionTercero_TUsuario_SupervisorId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210224164150_TParametroLiquidacionTercero_DropColumnSupervisor')
BEGIN
    DROP INDEX [IX_TParametroLiquidacionTercero_SupervisorId] ON [TParametroLiquidacionTercero];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210224164150_TParametroLiquidacionTercero_DropColumnSupervisor')
BEGIN
    DECLARE @var53 sysname;
    SELECT @var53 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TParametroLiquidacionTercero]') AND [c].[name] = N'SupervisorId');
    IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [TParametroLiquidacionTercero] DROP CONSTRAINT [' + @var53 + '];');
    ALTER TABLE [TParametroLiquidacionTercero] DROP COLUMN [SupervisorId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210224164150_TParametroLiquidacionTercero_DropColumnSupervisor')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210224164150_TParametroLiquidacionTercero_DropColumnSupervisor', N'5.0.0');
END;
GO

COMMIT;
GO
