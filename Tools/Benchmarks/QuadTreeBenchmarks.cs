using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Chaos.Collections.Specialized;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;

namespace Benchmarks;

[MemoryDiagnoser]
public class QuadTreeBenchmarks
{
    private Rectangle Bounds = null!;
    public int Capacity = 32;
    private Consumer Consumer = null!;

    [Params(1000, 2500, 5000)]
    public int EntityCount;

    private HashSet<int>[,] PointLookup = null!;
    private QuadTree<Point> Tree = null!;

    private IEnumerable<int> InnerSearchCircle()
    {
        var point = Bounds.GetRandomPoint();

        foreach (var pt in point.SpiralSearch(15))
        {
            if (!Bounds.Contains(pt))
                continue;

            var entities = PointLookup[pt.X, pt.Y];

            if (entities.Count == 0)
                continue;

            foreach (var entity in entities)
                yield return entity;
        }
    }

    [Benchmark]
    public void SearchLookupByCircle()
    {
        var ret = InnerSearchCircle()
            .ToList();

        Consumer.Consume(ret);
    }

    //[Benchmark]
    public void SearchPoint()
    {
        var point = Bounds.GetRandomPoint();

        var ret = Tree.Query(point)
                      .ToList();

        Consumer.Consume(ret);
    }

    [Benchmark]
    public void SearchQuadByCircle()
    {
        var point = Bounds.GetRandomPoint();
        var circle = new Circle(point, 15);

        var ret = Tree.Query(circle)
                      .ToList();

        Consumer.Consume(ret);
    }

    [GlobalSetup]
    public void Setup()
    {
        QuadTree<Point>.Capacity = Capacity;
        Consumer = new Consumer();

        Bounds = new Rectangle(
            0,
            0,
            255,
            255);
        Tree = new QuadTree<Point>(Bounds);
        PointLookup = new HashSet<int>[255, 255];

        for (var x = 0; x < 255; x++)
            for (var y = 0; y < 255; y++)
                PointLookup[x, y] = [];

        var points = new HashSet<Point>();

        for (var i = 0; i < EntityCount; i++)
            if (Bounds.TryGetRandomPoint(pt => points.Add(pt), out var point))
            {
                Tree.Insert(point.Value);

                PointLookup[point.Value.X, point.Value.Y]
                    .Add(point.Value.GetHashCode());
            }
    }
}