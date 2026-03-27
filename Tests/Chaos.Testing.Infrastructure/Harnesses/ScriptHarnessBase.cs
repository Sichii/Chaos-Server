#region
using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Harnesses;

/// <summary>
///     Base class for script test harnesses. Provides shared map, source, and target setup.
/// </summary>
public abstract class ScriptHarnessBase<THarness> where THarness: ScriptHarnessBase<THarness>
{
    /// <summary>
    ///     The shared map instance for this harness
    /// </summary>
    public MapInstance Map { get; private set; }

    /// <summary>
    ///     The service provider used for script instantiation
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    ///     The source aisling (the one using the skill/spell/item)
    /// </summary>
    public Aisling Source { get; private set; }

    /// <summary>
    ///     The mock client for the source aisling, for verification
    /// </summary>
    public Mock<IChaosWorldClient> SourceClient => Mock.Get(Source.Client);

    protected ScriptHarnessBase(IServiceProvider? serviceProvider = null)
    {
        Map = MockMapInstance.Create(width: 20, height: 20);

        Source = MockAisling.Create(
            Map,
            setup: a =>
            {
                a.UserStatSheet.SetMaxWeight(500);
            });

        ServiceProvider = serviceProvider ?? CreateDefaultServiceProvider();
    }

    private static IServiceProvider CreateDefaultServiceProvider()
        => MockServiceProvider.CreateBuilder()
                              .Build()
                              .Object;

    /// <summary>
    ///     Creates a script instance using ActivatorUtilities, passing the subject as an explicit parameter
    /// </summary>
    protected TScript CreateScript<TScript>(object subject) where TScript: class
        => ActivatorUtilities.CreateInstance<TScript>(ServiceProvider, subject);

    /// <summary>
    ///     Replace the map
    /// </summary>
    public THarness WithMap(MapInstance map)
    {
        Map = map;

        return (THarness)this;
    }

    /// <summary>
    ///     Replace the service provider used for script instantiation
    /// </summary>
    public THarness WithServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        return (THarness)this;
    }

    /// <summary>
    ///     Replace the source with a custom aisling
    /// </summary>
    public THarness WithSource(Aisling source)
    {
        Source = source;

        return (THarness)this;
    }

    /// <summary>
    ///     Configure the source aisling
    /// </summary>
    public THarness WithSource(Action<Aisling> setup)
    {
        setup(Source);

        return (THarness)this;
    }
}