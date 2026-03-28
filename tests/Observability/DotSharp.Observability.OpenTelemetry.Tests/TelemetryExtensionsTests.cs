using System.Diagnostics;
using DotSharp.Observability.Tracing;
using FluentAssertions;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Xunit;

namespace DotSharp.Observability.OpenTelemetry.Tests;

public sealed class TelemetryExtensionsTests
{
    [Fact]
    public void AddDotSharpInstrumentation_BuildsTracerProviderSuccessfully()
    {
        List<Activity> exportedActivities = [];

        TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddDotSharpInstrumentation()
            .AddInMemoryExporter(exportedActivities)
            .Build();

        tracerProvider.Should().NotBeNull();
        tracerProvider.Dispose();
    }

    [Fact]
    public void AddDotSharpInstrumentation_CapturesActivitiesFromDotSharpSource()
    {
        List<Activity> exportedActivities = [];

        using TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddDotSharpInstrumentation()
            .AddInMemoryExporter(exportedActivities)
            .Build();

        using (Activity? activity = DotSharpActivitySource.Instance.StartActivity("TestOperation"))
        {
            activity.Should().NotBeNull();
        }

        exportedActivities.Should().ContainSingle()
            .Which.DisplayName.Should().Be("TestOperation");
    }
}
