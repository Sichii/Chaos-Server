#region
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class RentedTests
{
    [Test]
    public void Array_HasCorrectCountAndZeroOffset()
    {
        using var rented = new Rented<int>(7);

        rented.Array
              .Count
              .Should()
              .Be(7);

        rented.Array
              .Offset
              .Should()
              .Be(0);
    }

    [Test]
    public void Constructor_SetsCountCorrectly()
    {
        using var rented = new Rented<int>(10);

        rented.Count
              .Should()
              .Be(10);
    }

    [Test]
    public void Constructor_ZeroSize_SpanIsEmpty()
    {
        using var rented = new Rented<int>(0);

        rented.Count
              .Should()
              .Be(0);

        rented.Span
              .Length
              .Should()
              .Be(0);
    }

    [Test]
    public void Deconstruct_YieldsSpanAndArray()
    {
        using var rented = new Rented<byte>(4);
        rented.Span[0] = 42;

        (var span, var array) = rented;

        span.Length
            .Should()
            .Be(4);

        span[0]
            .Should()
            .Be(42);

        array.Should()
             .NotBeNull();
    }

    [Test]
    public void Dispose_DoesNotThrow()
    {
        var rented = new Rented<int>(3);

        var act = () => rented.Dispose();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void ImplicitConversion_ToSpan_Works()
    {
        using var rented = new Rented<int>(3);
        rented.Span[0] = 99;

        Span<int> span = rented;

        span.Length
            .Should()
            .Be(3);

        span[0]
            .Should()
            .Be(99);
    }

    [Test]
    public void MultipleRentals_DoNotInterfere()
    {
        using var rented1 = new Rented<int>(5);
        using var rented2 = new Rented<int>(10);

        rented1.Count
               .Should()
               .Be(5);

        rented2.Count
               .Should()
               .Be(10);
    }

    [Test]
    public void Span_HasCorrectLength()
    {
        using var rented = new Rented<int>(5);

        rented.Span
              .Length
              .Should()
              .Be(5);
    }

    [Test]
    public void WorksWithReferenceTypes_CanStoreAndRetrieveObjects()
    {
        using var rented = new Rented<string>(3);
        rented.Span[0] = "hello";
        rented.Span[1] = "world";
        rented.Span[2] = "!";

        rented.Count
              .Should()
              .Be(3);

        rented.Span[0]
              .Should()
              .Be("hello");

        rented.Span[2]
              .Should()
              .Be("!");
    }
}