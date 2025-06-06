using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleTube.RestApi.Infrastructure.Database.Entities;

namespace SimpleTube.RestApi.Infrastructure.Database;

internal static class Extensions
{
    public static void Audit(this EntityEntry<AuditableEntity> entry)
    {
        var now = DateTimeOffset.Now;

        entry.Entity.LastModifiedAt = now;

        if (entry.State != EntityState.Added)
            return;

        entry.Entity.CreatedAt = now;
    }

    public static bool IsChanged(this EntityState state) =>
        state != EntityState.Detached && state != EntityState.Unchanged;
}
