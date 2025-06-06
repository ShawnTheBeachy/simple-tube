using Microsoft.EntityFrameworkCore;

namespace SimpleTube.RestApi.Infrastructure.Database;

internal static class DependencyInjection
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    ) =>
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlite(configuration.GetConnectionString("Database"))
        );
}
