#region
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
#endregion

namespace Benchmarks;

/// <summary>
///     Benchmarks comparing separate implementations vs generic approaches for Point extension methods. Tests whether
///     generics can avoid boxing while reducing code duplication. This benchmark uses C# 13's 'allows ref struct'
///     constraint to enable ValuePoint (a ref struct) to be used with generic methods. This tests whether a single generic
///     implementation can replace all 9 type-specific overloads (Point/ValuePoint/IPoint × Point/ValuePoint/IPoint)
///     without sacrificing performance.
/// </summary>
[InliningDiagnoser(
    false,
    [
        "Benchmarks",
        "Chaos.Extensions.Geometry"
    ])]
[DisassemblyDiagnoser(3)]
public class PointExtensionsBenchmarks
{
    private IPoint InterfacePoint1 = null!;
    private IPoint InterfacePoint2 = null!;
    private Point Point1;
    private Point Point2;

    [GlobalSetup]
    public void Setup()
    {
        Point1 = new Point(10, 20);
        Point2 = new Point(50, 80);
        InterfacePoint1 = new Point(10, 20);
        InterfacePoint2 = new Point(50, 80);
    }

    #region ManhattanDistance Benchmarks
    [Benchmark(Baseline = true)]
    public int Current_PointPoint() => Point1.ManhattanDistanceFrom(Point2);

    [Benchmark]
    public int Current_ValuePointValuePoint()
    {
        ValuePoint vp1 = new(10, 20);
        ValuePoint vp2 = new(50, 80);

        return vp1.ManhattanDistanceFrom(vp2);
    }

    [Benchmark]
    public int Current_IPointIPoint() => InterfacePoint1.ManhattanDistanceFrom(InterfacePoint2);

    [Benchmark]
    public int Generic_PointPoint() => ManhattanDistanceFromGeneric(Point1, Point2);

    [Benchmark]
    public int Generic_IPointIPoint() => ManhattanDistanceFromGeneric(InterfacePoint1, InterfacePoint2);

    [Benchmark]
    public int GenericInlined_PointPoint() => ManhattanDistanceFromGenericInlined(Point1, Point2);

    [Benchmark]
    public int GenericInlined_IPointIPoint() => ManhattanDistanceFromGenericInlined(InterfacePoint1, InterfacePoint2);

    [Benchmark]
    public int Generic_ValuePointValuePoint()
    {
        ValuePoint vp1 = new(10, 20);
        ValuePoint vp2 = new(50, 80);

        return ManhattanDistanceFromGeneric(vp1, vp2);
    }

    [Benchmark]
    public int GenericInlined_ValuePointValuePoint()
    {
        ValuePoint vp1 = new(10, 20);
        ValuePoint vp2 = new(50, 80);

        return ManhattanDistanceFromGenericInlined(vp1, vp2);
    }
    #endregion

    #region Generic Implementations
    // Without AggressiveInlining - tests if JIT naturally optimizes this
    // C# 13 'allows ref struct' enables ValuePoint (ref struct) to be used with generics
    public static int ManhattanDistanceFromGeneric<T>(T point, T other) where T: IPoint, allows ref struct
        => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    // With AggressiveInlining - tests if explicit inlining hint helps
    // C# 13 'allows ref struct' enables ValuePoint (ref struct) to be used with generics
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFromGenericInlined<T>(T point, T other) where T: IPoint, allows ref struct
        => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
    #endregion
}