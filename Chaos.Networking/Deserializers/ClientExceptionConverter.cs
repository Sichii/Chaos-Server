using System.Text;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ClientExceptionArgs" />
/// </summary>
public sealed class ClientExceptionConverter : PacketConverterBase<ClientExceptionArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ClientException;

    /// <inheritdoc />
    public override ClientExceptionArgs Deserialize(ref SpanReader reader)
    {
        var data = reader.ReadData();

        return new ClientExceptionArgs
        {
            ExceptionStr = Encoding.GetEncoding(949)
                                   .GetString(data)
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ClientExceptionArgs args)
        =>

            //LI: need to get the real layout of this packet
            writer.WriteData(
                Encoding.GetEncoding(949)
                        .GetBytes(args.ExceptionStr));
}