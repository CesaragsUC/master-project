namespace Domain.Interfaces;

public interface IMigratorService
{
    Task Run();
    Task RollbackMigration(long targetVersion);
    Task RollbackAllMigrations();
    Task RollbackLastMigrations(int steps);
    Task EnsureDatabaseExists();
}
