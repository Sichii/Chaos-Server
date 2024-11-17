#region
using FluentAssertions;
#endregion

namespace Chaos.Testing.Infrastructure;

public class DummyTestClass
{
    [Test]
    public void DummyTest()
        => true.Should()
               .BeTrue();
}