using Microsoft.EntityFrameworkCore;

namespace SimpleTube.RestApi.Infrastructure.Database;

internal sealed class AppDbContext : DbContext
{
    public DbSet<Entities.SubscriptionEntity> Subscriptions { get; init; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.SubscriptionEntity>(entityBuilder =>
        {
            entityBuilder.ToTable("Subscriptions");
            entityBuilder.HasKey(x => x.ChannelId);
        });
    }
}
