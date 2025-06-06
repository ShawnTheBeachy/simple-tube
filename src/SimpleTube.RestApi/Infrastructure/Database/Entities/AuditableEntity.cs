namespace SimpleTube.RestApi.Infrastructure.Database.Entities;

public abstract class AuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastModifiedAt { get; set; }
}
