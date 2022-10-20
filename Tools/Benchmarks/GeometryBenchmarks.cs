using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;

namespace Benchmarks;

[MemoryDiagnoser]
public class GeometryBenchmarks
{
    public Point CenterPoint = new(50, 50);
    [Params(
        3,
        6,
        9,
        13)]
    public int Range;

    [Benchmark(Baseline = true)]
    public void Rectangle()
    {
        var rect = new Rectangle(CenterPoint, Range * 2 + 1, Range * 2 + 1);

        var points = rect.Points()
                         .Where(p => p.DistanceFrom(CenterPoint) <= Range)
                         .ToList();
    }

    [Benchmark]
    public void SpiralSearch()
    {
        var points = CenterPoint.SpiralSearch(Range)
                                .ToList();
    }
}