namespace Chaos.Testing.Infrastructure;

//This class is required to have this project output a test assembly
//The test assembly is needed because the report generator looks for it because this is technically a "test project"
public sealed class Dummy
{
    [Test]
    public void DummyTest()
    {
        //dummy test
    }
}