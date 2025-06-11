using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimpleTube.RestApi.Infrastructure.Database.Compiled;
using SimpleTube.RestApi.Infrastructure.Database.Interceptors;

namespace SimpleTube.RestApi.Infrastructure.Database;

internal static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        var dbPath = AppData.GetFile("app.db");
        var connectionString = $"Data Source={dbPath}";
        EnsureDatabaseFileExists(dbPath, connectionString);
        return services
            .AddDbContext<AppDbContext>(
                (sp, opts) =>
                    opts.UseSqlite(connectionString)
                        .AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>())
                        .UseModel(AppDbContextModel.Instance)
            )
            .AddTransient<AuditingSaveChangesInterceptor>()
            .AddSingleton(new ConnectionStringProvider { ConnectionString = connectionString })
            .AddTransient<IDbConnectionFactory, SqliteConnectionFactory>();
    }

    private static void EnsureDatabaseFileExists(string filePath, string connectionString)
    {
        var fileInfo = new FileInfo(filePath);
        Directory.CreateDirectory(fileInfo.DirectoryName!);

        if (File.Exists(filePath))
            return;

        var migrationsSql = File.ReadAllText("Infrastructure/Database/Migrations/migrations.sql");

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = migrationsSql;
        command.ExecuteNonQuery();
    }
}
