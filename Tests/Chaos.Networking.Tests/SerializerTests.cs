using System.Text;
using Chaos.Networking.Model.Server;
using Chaos.Packets;
using Xunit;

namespace Chaos.Networking.Tests;

public class SerializerTests
{
    private readonly Encoding Encoding;

    public SerializerTests()
    {
        var provider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(provider);
        Encoding = provider.GetEncoding(949)!;
    }
    
}