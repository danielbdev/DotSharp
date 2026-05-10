using DotSharp.Persistence.EFCore.Outbox;
using DotSharp.Primitives.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DotSharp.Persistence.EFCore.Interceptors;

/// <summary>
/// SaveChanges interceptor that converts domain events to outbox messages.
/// Clears domain events from entities after a successful save.
/// </summary>
public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly ConditionalWeakTable<DbContext, List<IHasDomainEvents>> _tracked = [];

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? db = eventData.Context;

        if (db is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        List<IHasDomainEvents> entities = [.. db.ChangeTracker.Entries()
            .Where(e => e.Entity is IHasDomainEvents)
            .Select(e => (IHasDomainEvents)e.Entity)];

        if (entities.Count == 0)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        List<DomainEvent> events = [.. entities.SelectMany(e => e.DomainEvents)];

        if (events.Count == 0)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (DomainEvent evt in events)
        {
            Type evtType = evt.GetType();
            (string name, int version) = GetStableKey(evtType);

            string payload = JsonSerializer.Serialize(evt, evtType, _jsonOptions);

            db.Set<OutboxMessage>().Add(new OutboxMessage
            {
                Id = evt.EventId,
                OccurredOnUtc = evt.OccurredOnUtc,
                CorrelationId = evt.CorrelationId,
                EventName = name,
                EventVersion = version,
                Payload = payload,
                AttemptCount = 0
            });
        }

        _tracked.Remove(db);
        _tracked.Add(db, entities);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } db && _tracked.TryGetValue(db, out List<IHasDomainEvents>? entities))
        {
            foreach (IHasDomainEvents entity in entities)
                entity.ClearDomainEvents();

            _tracked.Remove(db);
        }

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc />
    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } db)
            _tracked.Remove(db);

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private static (string name, int version) GetStableKey(Type eventType)
    {
        OutboxEventAttribute? attr = eventType
            .GetCustomAttributes(typeof(OutboxEventAttribute), false)
            .Cast<OutboxEventAttribute>()
            .SingleOrDefault();

        return attr is not null
            ? (attr.Name, attr.Version)
            : (eventType.FullName ?? eventType.Name, 1);
    }
}
