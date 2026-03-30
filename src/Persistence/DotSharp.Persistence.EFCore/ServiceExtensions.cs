using DotSharp.Persistence.Abstractions.Repositories;
using DotSharp.Persistence.Abstractions.Specifications;
using DotSharp.Persistence.Abstractions.UnitOfWork;
using DotSharp.Persistence.EFCore.AuditLog;
using DotSharp.Persistence.EFCore.Repositories;
using DotSharp.Persistence.EFCore.Specifications;
using DotSharp.Persistence.EFCore.UnitOfWork;
using DotSharp.Primitives.AuditLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotSharp.Persistence.EFCore;

/// <summary>
/// Extension methods for registering DotSharp EF Core persistence services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers the EF Core persistence services including repositories, specification evaluator and unit of work.
    /// </summary>
    /// <typeparam name="TContext">The application DbContext type.</typeparam>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddDotSharpPersistenceEFCore<TContext>(
        this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());

        services.AddScoped<ISpecificationEvaluator, SpecificationEvaluator>();

        services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();

        services.AddScoped<IAuditLog, EfCoreAuditLog>();

        return services;
    }
}
