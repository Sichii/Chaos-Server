using Chaos.Extensions.Geometry;
using FluentAssertions;
using Xunit;

namespace Chaos.Geometry.Tests;

public class PointTests
{
    [Fact]
    public void SpiralSearchTest()
    {
        void RunTest(int range)
        {
            var point = new Point(50, 50);

            var points = point.SpiralSearch(range)
                              .ToList();

            var area = (int)Math.Ceiling(Math.Pow(range + range + 1, 2) / 2);

            points.Should()
                  .HaveCount(area);
        }

        RunTest(3);
        RunTest(6);
        RunTest(9);
        RunTest(13);
    }
}