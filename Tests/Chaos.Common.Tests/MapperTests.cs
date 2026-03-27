using Chaos.TypeMapper;
using Chaos.TypeMapper.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Common.Tests;

public class MapperTests
{
    [Test]
    public void Map_Generic_Should_Map_Object()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Foo, Bar>, FooToBarProfile>();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();

        var mapper = provider.GetRequiredService<ITypeMapper>();

        var result = mapper.Map<Foo, Bar>(
            new Foo
            {
                X = 7
            });

        result.X
              .Should()
              .Be(7);
    }

    [Test]
    public void Map_Generic_Should_Work_With_Reversed_Profile_Order()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Bar, Foo>, ReversedProfile>();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();

        var mapper = provider.GetRequiredService<ITypeMapper>();

        mapper.Map<Foo, Bar>(
                  new Foo
                  {
                      X = 3
                  })
              .X
              .Should()
              .Be(3);

        mapper.Map<Bar, Foo>(
                  new Bar
                  {
                      X = 4
                  })
              .X
              .Should()
              .Be(4);
    }

    [Test]
    public void Map_Object_Should_Map_Object()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Foo, Bar>, FooToBarProfile>();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();

        var mapper = provider.GetRequiredService<ITypeMapper>();

        var result = mapper.Map<Bar>(
            new Foo
            {
                X = 9
            });

        result.X
              .Should()
              .Be(9);
    }

    [Test]
    public void Map_Object_Should_Resolve_Via_Interface_Exploration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<IFooLike, Bar>, IFooLikeToBarProfile>();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();

        var mapper = provider.GetRequiredService<ITypeMapper>();

        var result = mapper.Map<Bar>(
            (object)new FooImpl
            {
                X = 11
            });

        result.X
              .Should()
              .Be(11);
    }

    [Test]
    public void Map_Should_Throw_When_No_Mapper_Found()
    {
        // Use unique types to avoid static resolver cache collisions across tests
        var services = new ServiceCollection();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        var act = () => mapper.Map<NoMapFoo, NoMapBar>(new NoMapFoo());

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void Map_Should_Throw_When_Null()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<ITypeMapper>();

        Action act = () => mapper.Map<Bar>((object)null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void MapMany_Generic_Should_Map_All_Items()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Foo, Bar>, FooToBarProfile>();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();

        var mapper = provider.GetRequiredService<ITypeMapper>();

        var foos = new[]
        {
            new Foo
            {
                X = 1
            },
            new Foo
            {
                X = 2
            }
        };

        var bars = mapper.MapMany<Foo, Bar>(foos)
                         .ToList();

        bars.Select(b => b.X)
            .Should()
            .Equal(1, 2);
    }

    [Test]
    public void MapMany_Object_Should_Map_All_Items_Using_First_Mapper()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMapperProfile<Foo, Bar>, FooToBarProfile>();
        services.AddSingleton<ITypeMapper, Mapper>();
        var provider = services.BuildServiceProvider();

        var mapper = provider.GetRequiredService<ITypeMapper>();

        var objs = new object[]
        {
            new Foo
            {
                X = 5
            },
            new Foo
            {
                X = 6
            }
        };

        var bars = mapper.MapMany<Bar>(objs)
                         .ToList();

        bars.Select(b => b.X)
            .Should()
            .Equal(5, 6);
    }

    private sealed class Bar
    {
        public int X { get; set; }
    }

    private sealed class Foo
    {
        public int X { get; set; }
    }

    private sealed class FooImpl : IFooLike
    {
        public int X { get; set; }
    }

    private sealed class FooToBarProfile : IMapperProfile<Foo, Bar>
    {
        public Foo Map(Bar obj)
            => new Foo
            {
                X = obj.X
            };

        public Bar Map(Foo obj)
            => new Bar
            {
                X = obj.X
            };
    }

    private interface IFooLike
    {
        int X { get; }
    }

    private sealed class IFooLikeToBarProfile : IMapperProfile<IFooLike, Bar>
    {
        public IFooLike Map(Bar obj)
            => new FooImpl
            {
                X = obj.X
            };

        public Bar Map(IFooLike obj)
            => new Bar
            {
                X = obj.X
            };
    }

    private sealed class NoMapBar { }

    private sealed class NoMapFoo { }

    private sealed class ReversedProfile : IMapperProfile<Bar, Foo>
    {
        public Bar Map(Foo obj)
            => new Bar
            {
                X = obj.X
            };

        public Foo Map(Bar obj)
            => new Foo
            {
                X = obj.X
            };
    }
}