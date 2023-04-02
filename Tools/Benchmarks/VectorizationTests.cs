using BenchmarkDotNet.Attributes;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;

namespace Benchmarks;

[MemoryDiagnoser]
public class VectorizationTests
{
    private readonly ICircle Circle;
    private readonly IPoint EndPoint;
    private readonly IPoint StartPoint;

    public VectorizationTests()
    {
        Circle = new Circle(new Point(15, 15), 10);
        StartPoint = new Point(0, 0);
        EndPoint = new Point(50, 50);
    }

    [Benchmark(Baseline = true)]
    public void Intersect()
    {
        var intsersection = Circle.CalculateIntersectionEntryPoint(StartPoint, EndPoint);
    }
}