using Xunit;

namespace Experiments;

public class RefStructExperiments
{
    public ref TestRefStruct DoThingsByRef(ref TestRefStruct trs)
    {
        trs.SomeProperty = 10;
        trs.SomeField = 5;

        var newTrs = new TestRefStruct
        {
            SomeProperty = 25,
            SomeField = 50
        };

        trs = newTrs;

        return ref trs;
    }

    [Fact]
    public void RefStructByRef()
    {
        var trs = new TestRefStruct
        {
            SomeProperty = 5,
            SomeField = 10
        };

        var newTrs = DoThingsByRef(ref trs);
    }

    public ref struct TestRefStruct
    {
        public int SomeField;
        public int SomeProperty { get; set; }
    }
}