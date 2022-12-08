using System.Text;

namespace Chaos.Networking.Tests;

public class SerializerTests
{
    public SerializerTests()
    {
        var provider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(provider);
    }
}