using DotSharp.Persistence.EFCore.Inbox;
using DotSharp.Persistence.EFCore.Outbox;
using Microsoft.EntityFrameworkCore;

namespace DotSharp.Persistence.EFCore.Contexts;

/// <summary>
/// Base DbContext for all business contexts.
/// Applies entity configurations from the derived context assembly.
/// Infrastructure configurations (Outbox/Inbox) must be applied by the provider package.
/// </summary>
public abstract class DotSharpDbContext(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// The outbox messages set.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    /// <summary>
    /// The inbox consumers set.
    /// </summary>
    public DbSet<InboxConsumer> InboxConsumers { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        ConfigureModel(modelBuilder);
    }

    /// <summary>
    /// Optional hook for derived contexts to apply additional model configuration.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected virtual void ConfigureModel(ModelBuilder modelBuilder) { }
}
