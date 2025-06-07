using Microsoft.EntityFrameworkCore;

namespace SimpleTube.RestApi.Infrastructure.Database;

internal sealed class AppDbContext : DbContext
{
    public DbSet<Entities.ChannelEntity> Channels { get; init; }
    public DbSet<Entities.VideoEntity> Videos { get; init; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.ChannelEntity>(entityBuilder =>
        {
            entityBuilder.ToTable("Channels");
            entityBuilder.HasKey(x => x.Id);
        });
        modelBuilder.Entity<Entities.VideoEntity>(entityBuilder =>
        {
            entityBuilder.ToTable("Videos");
            entityBuilder.HasKey(x => x.Id);
        });
    }
}
