using Microsoft.EntityFrameworkCore.Diagnostics;
using SimpleTube.RestApi.Infrastructure.Database.Entities;

namespace SimpleTube.RestApi.Infrastructure.Database.Interceptors;

internal sealed class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    private void Audit(DbContextEventData eventData)
    {
        foreach (
            var entry in eventData
                .Context!.ChangeTracker.Entries<AuditableEntity>()
                .Where(e => e.State.IsChanged())
        )
            entry.Audit();
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        Audit(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new()
    )
    {
        Audit(eventData);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
