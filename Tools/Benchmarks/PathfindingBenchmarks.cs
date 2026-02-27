#region
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
#endregion

namespace Benchmarks;

[MemoryDiagnoser]
public class PathfindingBenchmarks
{
    private Consumer Consumer;
    private Point End;
    private IPathfinder Pathfinder;
    private PathOptions PathOptions;
    private Point Start;

    private List<IPoint> Walls;

    [Benchmark]
    public void FindDirection()
    {
        var direction = Pathfinder.FindOptimalDirection(Start, End, PathOptions);
        Consumer.Consume(direction);
    }

    [Benchmark(Baseline = true)]
    public void FindPath()
    {
        var path = Pathfinder.FindPath(Start, End, PathOptions);
        Consumer.Consume(path);
    }

    [GlobalSetup]
    public void Setup()
    {
        Start = new Point(5, 5);
        End = new Point(95, 95);

        PathOptions = PathOptions.Default with
        {
            LimitRadius = 200
        };
        Consumer = new Consumer();

        Walls =
        [
            new Point(0, 0),
            new Point(1, 0),
            new Point(2, 0),
            new Point(3, 0),
            new Point(4, 0),
            new Point(5, 0),
            new Point(6, 0),
            new Point(7, 0),
            new Point(8, 0),
            new Point(9, 0),
            new Point(10, 0),
            new Point(20, 0),
            new Point(30, 0),
            new Point(40, 0),
            new Point(50, 0),
            new Point(60, 0),
            new Point(70, 0),
            new Point(80, 0),
            new Point(90, 0),
            new Point(99, 0),
            new Point(0, 99),
            new Point(1, 99),
            new Point(2, 99),
            new Point(3, 99),
            new Point(4, 99),
            new Point(5, 99),
            new Point(6, 99),
            new Point(7, 99),
            new Point(8, 99),
            new Point(9, 99),
            new Point(10, 99),
            new Point(20, 99),
            new Point(30, 99),
            new Point(40, 99),
            new Point(50, 99),
            new Point(60, 99),
            new Point(70, 99),
            new Point(80, 99),
            new Point(90, 99),
            new Point(99, 99),

            // Left & Right

            new Point(0, 10),
            new Point(0, 20),
            new Point(0, 30),
            new Point(0, 40),
            new Point(0, 50),
            new Point(0, 60),
            new Point(0, 70),
            new Point(0, 80),
            new Point(0, 90),
            new Point(99, 10),
            new Point(99, 20),
            new Point(99, 30),
            new Point(99, 40),
            new Point(99, 50),
            new Point(99, 60),
            new Point(99, 70),
            new Point(99, 80),
            new Point(99, 90),

            // ===== Vertical Wall (gap at y=50-52) =====

            new Point(45, 10),
            new Point(45, 11),
            new Point(45, 12),
            new Point(45, 13),
            new Point(45, 14),
            new Point(45, 15),
            new Point(45, 16),
            new Point(45, 17),
            new Point(45, 18),
            new Point(45, 19),
            new Point(45, 20),
            new Point(45, 21),
            new Point(45, 22),
            new Point(45, 23),
            new Point(45, 24),
            new Point(45, 25),
            new Point(45, 26),
            new Point(45, 27),
            new Point(45, 28),
            new Point(45, 29),
            new Point(45, 30),
            new Point(45, 31),
            new Point(45, 32),
            new Point(45, 33),
            new Point(45, 34),
            new Point(45, 35),
            new Point(45, 36),
            new Point(45, 37),
            new Point(45, 38),
            new Point(45, 39),
            new Point(45, 40),
            new Point(45, 41),
            new Point(45, 42),
            new Point(45, 43),
            new Point(45, 44),
            new Point(45, 45),
            new Point(45, 46),
            new Point(45, 47),
            new Point(45, 48),
            new Point(45, 49),

            // gap here (50-52)

            new Point(45, 53),
            new Point(45, 54),
            new Point(45, 55),
            new Point(45, 56),
            new Point(45, 57),
            new Point(45, 58),
            new Point(45, 59),
            new Point(45, 60),
            new Point(45, 61),
            new Point(45, 62),
            new Point(45, 63),
            new Point(45, 64),
            new Point(45, 65),
            new Point(45, 66),
            new Point(45, 67),
            new Point(45, 68),
            new Point(45, 69),
            new Point(45, 70),
            new Point(45, 71),
            new Point(45, 72),
            new Point(45, 73),
            new Point(45, 74),
            new Point(45, 75),
            new Point(45, 76),
            new Point(45, 77),
            new Point(45, 78),
            new Point(45, 79),
            new Point(45, 80),

            // ===== Horizontal Wall (gap at x=20-22) =====

            new Point(10, 60),
            new Point(11, 60),
            new Point(12, 60),
            new Point(13, 60),
            new Point(14, 60),
            new Point(15, 60),
            new Point(16, 60),
            new Point(17, 60),
            new Point(18, 60),
            new Point(19, 60),

            // gap (20-22)

            new Point(23, 60),
            new Point(24, 60),
            new Point(25, 60),
            new Point(26, 60),
            new Point(27, 60),
            new Point(28, 60),
            new Point(29, 60),
            new Point(30, 60),
            new Point(31, 60),
            new Point(32, 60),
            new Point(33, 60),
            new Point(34, 60),
            new Point(35, 60),
            new Point(36, 60),
            new Point(37, 60),
            new Point(38, 60),
            new Point(39, 60),
            new Point(40, 60),
            new Point(41, 60),
            new Point(42, 60),

            // ===== Boxed Room =====

            new Point(70, 20),
            new Point(71, 20),
            new Point(72, 20),
            new Point(73, 20),
            new Point(74, 20),
            new Point(75, 20),
            new Point(76, 20),
            new Point(77, 20),
            new Point(78, 20),
            new Point(79, 20),
            new Point(70, 30),
            new Point(71, 30),
            new Point(72, 30),
            new Point(73, 30),
            new Point(74, 30),
            new Point(75, 30),
            new Point(76, 30),
            new Point(77, 30),
            new Point(78, 30),
            new Point(79, 30),
            new Point(70, 21),
            new Point(70, 22),
            new Point(70, 23),
            new Point(70, 24),
            new Point(70, 25),
            new Point(70, 26),
            new Point(70, 27),
            new Point(70, 28),
            new Point(70, 29),
            new Point(79, 21),
            new Point(79, 22),
            new Point(79, 23),
            new Point(79, 24),
            new Point(79, 25),
            new Point(79, 26),
            new Point(79, 27),
            new Point(79, 28),
            new Point(79, 29),

            // ===== Diagonal Barrier =====

            new Point(10, 10),
            new Point(11, 11),
            new Point(12, 12),
            new Point(13, 13),
            new Point(14, 14),
            new Point(15, 15),
            new Point(16, 16),
            new Point(17, 17),
            new Point(18, 18),
            new Point(19, 19),
            new Point(20, 20),
            new Point(21, 21),
            new Point(22, 22),
            new Point(23, 23),
            new Point(24, 24),
            new Point(25, 25),

            // ===== Scattered Obstacles =====

            new Point(12, 75),
            new Point(14, 77),
            new Point(16, 74),
            new Point(88, 88),
            new Point(87, 89),
            new Point(89, 87),
            new Point(55, 40),
            new Point(56, 41),
            new Point(57, 39),
            new Point(33, 15),
            new Point(34, 16),
            new Point(35, 17)
        ];

        var gridDetails = new GridDetails
        {
            Width = 100,
            Height = 100,
            Walls = Walls
        };

        Pathfinder = new Pathfinder(gridDetails);
    }
}