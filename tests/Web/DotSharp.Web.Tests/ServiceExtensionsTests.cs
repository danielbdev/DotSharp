using DotSharp.Observability.Correlation;
using DotSharp.Observability.Tracing;
using DotSharp.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotSharp.Web.Tests;

/// <summary>
/// Tests for <see cref="ServiceExtensions.AddDotSharpWeb"/> service registration.
/// </summary>
public sealed class ServiceExtensionsTests
{
    [Fact]
    public void AddDotSharpWeb_RegistersHttpContextAccessor()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDotSharpWeb();

        // Assert
        Assert.Contains(services, sd => sd.ServiceType == typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
    }

    [Fact]
    public void AddDotSharpWeb_RegistersCorrelationContext_AsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDotSharpWeb();

        // Assert
        var registration = Assert.Single(services, sd => sd.ServiceType == typeof(ICorrelationContext));
        Assert.Equal(ServiceLifetime.Scoped, registration.Lifetime);
    }

    [Fact]
    public void AddDotSharpWeb_RegistersGlobalExceptionHandler_AsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDotSharpWeb();

        // Assert
        var registration = Assert.Single(services, sd => sd.ImplementationType?.Name == "GlobalExceptionHandler");
        Assert.Equal(ServiceLifetime.Scoped, registration.Lifetime);
    }

    [Fact]
    public void AddDotSharpWeb_RegistersValidationEndpointFilter_AsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDotSharpWeb();

        // Assert
        var registration = Assert.Single(services, sd => sd.ImplementationType?.Name == "ValidationEndpointFilter");
        Assert.Equal(ServiceLifetime.Transient, registration.Lifetime);
    }

    [Fact]
    public void AddDotSharpWeb_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDotSharpWeb();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddDotSharpWeb_CallsAddObservability_WhenTraceContextNotRegistered()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDotSharpWeb();

        // Assert
        Assert.Contains(services, sd => sd.ServiceType == typeof(ITraceContext));
    }

    [Fact]
    public void AddDotSharpWeb_DoesNotDuplicateObservability_WhenTraceContextAlreadyRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ITraceContext>(_ => new FakeTraceContext());
        var initialCount = services.Count;

        // Act
        services.AddDotSharpWeb();

        // Assert
        var traceRegistrations = services.Count(sd => sd.ServiceType == typeof(ITraceContext));
        Assert.Equal(1, traceRegistrations);
    }

    private sealed class FakeTraceContext : ITraceContext
    {
        public string TraceId => "fake-trace";
        public string SpanId => "fake-span";
    }
}
