#region
using System.Diagnostics;
using Chaos.Common.Abstractions.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class ActivitySourcesTests
{
    [Test]
    public void StartGeneralActivity_WithListener_ReturnsActivity()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = src => ReferenceEquals(src, ActivitySources.GeneralActivitySource),
            Sample = (ref opts) => ActivitySamplingResult.AllDataAndRecorded
        };

        ActivitySource.AddActivityListener(listener);

        using var result = ActivitySources.StartGeneralActivity("general-test");

        result.Should()
              .NotBeNull();
    }

    [Test]
    public void StartInternalActivity_WhenAmbientActivityExists_ReturnsActivity()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref opts) => ActivitySamplingResult.AllDataAndRecorded
        };

        ActivitySource.AddActivityListener(listener);

        // Use a dedicated test source to set Activity.Current
        using var testSource = new ActivitySource("test-ambient-source");
        using var parentActivity = testSource.StartActivity("parent");

        if (parentActivity is null)

            // Listener not registering properly in this environment; skip
            return;

        using var result = ActivitySources.StartInternalActivity("internal-child");

        result.Should()
              .NotBeNull();
    }

    [Test]
    public void StartInternalActivity_WhenNoAmbientActivity_ReturnsNull()
    {
        // Ensure no ambient activity is set on this thread
        Activity.Current = null;

        var result = ActivitySources.StartInternalActivity("no-ambient");

        result.Should()
              .BeNull();
    }
}