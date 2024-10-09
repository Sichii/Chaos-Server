using Chaos.Cryptography;
using Chaos.Extensions.Common;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="LoginArgs" />
/// </summary>
public sealed class LoginConverter : PacketConverterBase<LoginArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Login;

    /// <inheritdoc />
    public override LoginArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();

        return new LoginArgs
        {
            Name = name,
            Password = pw
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LoginArgs args)
    {
        writer.WriteString8(args.Name);
        writer.WriteString8(args.Password);

        //generate random keys
        var xorInverseSource = Random.Shared.Next<byte>(0, byte.MaxValue);
        var saltSource = Random.Shared.Next<byte>(0, byte.MaxValue);

        //generate randomIdA and salt
        var randomIdA = Random.Shared.Next<uint>(0, uint.MaxValue);
        var randomIdASalt = (uint)(byte)(saltSource + 138);

        //generate randomIdB and salt
        var randomIdB = Random.Shared.Next<uint>(0, uint.MaxValue);
        var randomIdBSalt = (uint)(byte)(saltSource + 115);

        //get checksum and salt
        //intentionally little endian
        var checkSum = Crc.Generate16(BitConverter.GetBytes(randomIdA));
        var checksumSalt = (uint)(byte)(saltSource + 94);

        randomIdA ^= randomIdASalt | ((randomIdASalt + 1) << 8) | ((randomIdASalt + 2) << 16) | ((randomIdASalt + 3) << 24);
        checkSum ^= (ushort)(checksumSalt | ((checksumSalt + 1) << 8));
        randomIdB ^= randomIdBSalt | ((randomIdBSalt + 1) << 8) | ((randomIdBSalt + 2) << 16) | ((randomIdBSalt + 3) << 24);

        writer.WriteByte(xorInverseSource);
        writer.WriteByte((byte)(saltSource ^ (xorInverseSource + 59)));
        writer.WriteUInt32(randomIdA);
        writer.WriteUInt16(checkSum);
        writer.WriteUInt32(randomIdB);

        var buffer = writer.ToSpan();

        //get a checksum of the key data that has been written thusfar
        var keyBuffer = buffer[(args.Name.Length + args.Password.Length + 2)..];
        var keyChecksum = Crc.Generate16(keyBuffer);

        writer.WriteUInt16(keyChecksum);
        writer.WriteUInt16(256);
    }
}