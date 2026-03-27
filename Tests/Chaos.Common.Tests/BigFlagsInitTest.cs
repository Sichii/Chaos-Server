#region
using Chaos.Testing.Infrastructure.Definitions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsInitTest
{
    [Test]
    public void DebugInit()
    {
        try
        {
            var val = TestFeatures.Feature1;
            Console.WriteLine($"Feature1 value: {val.Value}");
        } catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex}");

            throw;
        }
    }
}