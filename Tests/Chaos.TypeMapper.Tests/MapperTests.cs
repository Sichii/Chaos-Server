#region
using Chaos.TypeMapper.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassCanBeSealed.Global
#endregion

namespace Chaos.TypeMapper.Tests;

public sealed class MapperTests
{
    private static ITypeMapper CreateMapper(IServiceCollection? services = null)
    {
        services ??= new ServiceCollection();

        services.AddSingleton<ITypeMapper, Mapper>();

        var provider = services.BuildServiceProvider();

        return provider.GetRequiredService<ITypeMapper>();
    }

    [Test]
    public void Map_CachesResolver_SecondCallSucceeds()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var source = new Source
        {
            Name = "cached",
            Value = 1
        };

        var result1 = mapper.Map<Source, Dest>(source);
        var result2 = mapper.Map<Source, Dest>(source);

        result1.Name
               .Should()
               .Be("cached");

        result2.Name
               .Should()
               .Be("cached");
    }

    [Test]
    public void Map_DerivedType_FindsBaseTypeProfile()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var derived = new DerivedSource
        {
            Name = "derived",
            Value = 55,
            Extra = "extra"
        };

        var result = mapper.Map<Dest>(derived);

        result.Name
              .Should()
              .Be("derived");

        result.Value
              .Should()
              .Be(55);
    }

    [Test]
    public void Map_GenericTwoTypeParams_NullObj_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var act = () => mapper.Map<Source, Dest>(null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Map_InterfaceType_FindsProfile()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<ISource, Dest>, InterfaceTestProfile>();

        var mapper = CreateMapper(services);

        var source = new Source
        {
            Name = "ifacetest",
            Value = 77
        };

        var result = mapper.Map<Dest>(source);

        result.Name
              .Should()
              .Be("ifacetest");

        result.Value
              .Should()
              .Be(77);
    }

    [Test]
    public void Map_NoProfile_ThrowsInvalidOperationException()
    {
        var mapper = CreateMapper();

        var act = () => mapper.Map<Source, Dest>(
            new Source
            {
                Name = "x",
                Value = 1
            });

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void Map_ReverseDirection_MapsCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var dest = new Dest
        {
            Name = "reverse",
            Value = 99
        };

        var result = mapper.Map<Dest, Source>(dest);

        result.Name
              .Should()
              .Be("reverse");

        result.Value
              .Should()
              .Be(99);
    }

    [Test]
    public void Map_SingleTypeParam_MapsCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var source = new Source
        {
            Name = "test",
            Value = 7
        };

        var result = mapper.Map<Dest>(source);

        result.Name
              .Should()
              .Be("test");

        result.Value
              .Should()
              .Be(7);
    }

    [Test]
    public void Map_SingleTypeParam_NullObj_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var act = () => mapper.Map<Dest>(null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Map_WithRegisteredProfile_MapsCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var source = new Source
        {
            Name = "hello",
            Value = 42
        };

        var result = mapper.Map<Source, Dest>(source);

        result.Name
              .Should()
              .Be("hello");

        result.Value
              .Should()
              .Be(42);
    }

    [Test]
    public void MapMany_EmptyCollection_YieldsNothing()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var results = mapper.MapMany<Source, Dest>([])
                            .ToList();

        results.Should()
               .BeEmpty();
    }

    [Test]
    public void MapMany_GenericTwoTypeParams_MapsAllItems()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var sources = new[]
        {
            new Source
            {
                Name = "a",
                Value = 1
            },
            new Source
            {
                Name = "b",
                Value = 2
            }
        };

        var results = mapper.MapMany<Source, Dest>(sources)
                            .ToList();

        results.Should()
               .HaveCount(2);

        results[0]
            .Name
            .Should()
            .Be("a");

        results[1]
            .Name
            .Should()
            .Be("b");
    }

    [Test]
    public void MapMany_ObjectOverload_MapsAllItems()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Source, Dest>, TestProfile>();

        var mapper = CreateMapper(services);

        var sources = new object[]
        {
            new Source
            {
                Name = "x",
                Value = 10
            },
            new Source
            {
                Name = "y",
                Value = 20
            }
        };

        var results = mapper.MapMany<Dest>(sources)
                            .ToList();

        results.Should()
               .HaveCount(2);

        results[0]
            .Value
            .Should()
            .Be(10);

        results[1]
            .Value
            .Should()
            .Be(20);
    }

    #region Test Types
    public interface ISource
    {
        string Name { get; }
        int Value { get; }
    }

    public class Source : ISource
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class DerivedSource : Source
    {
        public string Extra { get; set; } = string.Empty;
    }

    public class Dest
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private sealed class TestProfile : IMapperProfile<Source, Dest>
    {
        public Source Map(Dest obj)
            => new()
            {
                Name = obj.Name,
                Value = obj.Value
            };

        public Dest Map(Source obj)
            => new()
            {
                Name = obj.Name,
                Value = obj.Value
            };
    }

    private sealed class InterfaceTestProfile : IMapperProfile<ISource, Dest>
    {
        public ISource Map(Dest obj)
            => new Source
            {
                Name = obj.Name,
                Value = obj.Value
            };

        public Dest Map(ISource obj)
            => new()
            {
                Name = obj.Name,
                Value = obj.Value
            };
    }
    #endregion
}