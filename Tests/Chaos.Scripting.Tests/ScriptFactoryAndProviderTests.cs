#region
using System;
using System.Collections.Generic;
using Chaos.Scripting;
using Chaos.Scripting.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TUnit.Core;
#endregion

namespace Chaos.Scripting.Tests;

public sealed class ScriptFactoryAndProviderTests
{
    private ServiceProvider BuildProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddDebug()
                                              .SetMinimumLevel(LogLevel.Error));
        configure?.Invoke(services);

        return services.BuildServiceProvider();
    }

    [Test]
    public void ScriptFactory_WithMultipleKeys_ShouldSkipMismatchedTypesAndContinue()
    {
        var sp = BuildProvider();
        var logger = sp.GetRequiredService<ILogger<ScriptFactory<IDemoScript, DemoSubject>>>();
        var factory = new ScriptFactory<IDemoScript, DemoSubject>(logger, sp);

        var composite = factory.CreateScript(
            new[]
            {
                "LeafA",
                "LeafB"
            },
            new DemoSubject());
        var comp = (ICompositeScript<IDemoScript>)composite;

        comp.GetScript<LeafA>()
            .Should()
            .NotBeNull();

        comp.GetScript<LeafB>()
            .Should()
            .NotBeNull();
    }

    [Test]
    public void ScriptFactory_WithSingleKey_ShouldCreateLeafAndReturnComposite()
    {
        var sp = BuildProvider();
        var logger = sp.GetRequiredService<ILogger<ScriptFactory<IDemoScript, DemoSubject>>>();
        var factory = new ScriptFactory<IDemoScript, DemoSubject>(logger, sp);

        var composite = factory.CreateScript(
            new[]
            {
                "LeafA"
            },
            new DemoSubject());

        composite.Should()
                 .BeAssignableTo<ICompositeScript<IDemoScript>>();
        var script = ((ICompositeScript<IDemoScript>)composite).GetScript<LeafA>();

        script.Should()
              .NotBeNull();
    }

    [Test]
    public void ScriptFactory_WithUnknownKey_ShouldThrow()
    {
        var sp = BuildProvider();
        var logger = sp.GetRequiredService<ILogger<ScriptFactory<IDemoScript, DemoSubject>>>();
        var factory = new ScriptFactory<IDemoScript, DemoSubject>(logger, sp);

        Action act = () => factory.CreateScript(
            new[]
            {
                "DoesNotExist"
            },
            new DemoSubject());

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void ScriptProvider_ShouldResolveFactoryAndCreate()
    {
        var sp = BuildProvider(services =>
        {
            services.AddSingleton<IScriptFactory<IDemoScript, DemoSubject>>(sp2 =>
            {
                var logger = sp2.GetRequiredService<ILogger<ScriptFactory<IDemoScript, DemoSubject>>>();

                return new ScriptFactory<IDemoScript, DemoSubject>(logger, sp2);
            });
        });

        var provider = new ScriptProvider(sp);

        var script = provider.CreateScript<IDemoScript, DemoSubject>(
            new[]
            {
                "LeafA",
                "LeafB"
            },
            new DemoSubject());

        script.Should()
              .BeAssignableTo<ICompositeScript<IDemoScript>>();
        var comp = (ICompositeScript<IDemoScript>)script;

        comp.GetScript<LeafA>()
            .Should()
            .NotBeNull();

        comp.GetScript<LeafB>()
            .Should()
            .NotBeNull();
    }

    [Test]
    public void ScriptFactory_WithEmptyKeys_ReturnsEmptyComposite()
    {
        var sp = BuildProvider();
        var logger = sp.GetRequiredService<ILogger<ScriptFactory<IDemoScript, DemoSubject>>>();
        var factory = new ScriptFactory<IDemoScript, DemoSubject>(logger, sp);

        var composite = factory.CreateScript(Array.Empty<string>(), new DemoSubject());

        composite.Should()
                 .BeAssignableTo<ICompositeScript<IDemoScript>>();

        ((ICompositeScript<IDemoScript>)composite).GetScripts<IDemoScript>()
                                                  .Should()
                                                  .BeEmpty();
    }

    [Test]
    public void ScriptProvider_FactoryNotRegistered_ThrowsInvalidOperationException()
    {
        // Service provider has no factory registered
        var sp = BuildProvider();
        var provider = new ScriptProvider(sp);

        Action act = () => provider.CreateScript<IDemoScript, DemoSubject>(
            new[]
            {
                "LeafA"
            },
            new DemoSubject());

        act.Should()
           .Throw<InvalidOperationException>();
    }

    // The composite type must exist for the factory
    private sealed class CompositeDemoScript : CompositeScriptBase<IDemoScript>, IDemoScript { }

    private sealed class DemoSubject : IScripted
    {
        public ISet<string> ScriptKeys { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    private interface IDemoScript : IScript { }

    // Two leaf scripts need a ctor accepting the subject; the factory supplies the subject via ActivatorUtilities
    private sealed class LeafA : ScriptBase, IDemoScript
    {
        public LeafA(DemoSubject subject) { }
    }

    private sealed class LeafB : ScriptBase, IDemoScript
    {
        public LeafB(DemoSubject subject) { }
    }
}