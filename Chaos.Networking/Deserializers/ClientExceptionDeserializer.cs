using System.Text;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ClientExceptionArgs" />
/// </summary>
public sealed record ClientExceptionDeserializer : ClientPacketDeserializer<ClientExceptionArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.ClientException;

    /// <inheritdoc />
    public override ClientExceptionArgs Deserialize(ref SpanReader reader)
    {
        var data = reader.ReadData();

        return new ClientExceptionArgs(
            Encoding.GetEncoding(949)
                    .GetString(data));
    }
}