using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;

namespace Benchmarks;

[MemoryDiagnoser]

// ReSharper disable once ClassCanBeSealed.Global
public class GeometryBenchmarks
{
    public Point CenterPoint = new(50, 50);

    [Params(
        3,
        6,
        9,
        13)]

    // ReSharper disable once UnassignedField.Global
    public int Range;

    [Benchmark(Baseline = true)]
    public void Rectangle()
    {
        var rect = new Rectangle(CenterPoint, Range * 2 + 1, Range * 2 + 1);

        // ReSharper disable once UnusedVariable
        var points = rect.GetPoints()
                         .Where(p => p.ManhattanDistanceFrom(CenterPoint) <= Range)
                         .ToList();
    }

    [Benchmark]
    public void SpiralSearch()
    {
        // ReSharper disable once UnusedVariable
        var points = CenterPoint.SpiralSearch(Range)
                                .ToList();
    }
}