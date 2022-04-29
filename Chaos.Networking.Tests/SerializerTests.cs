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

    [Fact]
    public void PacketWriterTest()
    {
        var obj = new HeartBeatResponseArgs
        {
            First = 10,
            Second = 20
        };

        var serializer = new PacketSerializer(Encoding);
        var packet = serializer.Serialize(obj);
    }
}