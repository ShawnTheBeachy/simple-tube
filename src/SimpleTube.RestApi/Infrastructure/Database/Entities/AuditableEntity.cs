namespace SimpleTube.RestApi.Infrastructure.Database.Entities;

internal abstract class AuditableEntity
{
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset LastModifiedAt { get; init; }
}
