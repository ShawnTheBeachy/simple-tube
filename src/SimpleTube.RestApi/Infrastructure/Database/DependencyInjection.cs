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
        return services
            .AddDbContext<AppDbContext>(
                (sp, opts) =>
                    opts.UseSqlite(connectionString)
                        .AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>())
                        .UseModel(AppDbContextModel.Instance)
            )
            .AddTransient<AuditingSaveChangesInterceptor>()
            .AddSingleton(new ConnectionStringProvider { ConnectionString = connectionString });
    }
}
