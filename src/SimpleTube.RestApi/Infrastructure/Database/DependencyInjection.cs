using Microsoft.EntityFrameworkCore;
using SimpleTube.RestApi.Infrastructure.Database.Compiled;
using SimpleTube.RestApi.Infrastructure.Database.Interceptors;

namespace SimpleTube.RestApi.Infrastructure.Database;

internal static class DependencyInjection
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    ) =>
        services
            .AddDbContext<AppDbContext>(
                (sp, opts) =>
                    opts.UseSqlite(configuration.GetConnectionString("Database"))
                        .AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>())
                        .UseModel(AppDbContextModel.Instance)
            )
            .AddTransient<AuditingSaveChangesInterceptor>()
            .AddSingleton(
                new ConnectionStringProvider
                {
                    ConnectionString = configuration.GetConnectionString("Database")!,
                }
            );
}
