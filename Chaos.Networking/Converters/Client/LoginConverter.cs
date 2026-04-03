#region
using System.Buffers.Binary;
using Chaos.Cryptography;
using Chaos.Extensions.Common;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="LoginArgs" />
/// </summary>
public sealed class LoginConverter : PacketConverterBase<LoginArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Login;

    /// <summary>
    ///     Decodes client ids, the checksum of clientId1, and the integrity CRC from the encrypted payload
    /// </summary>
    public static void DecodeClientInfo(
        ReadOnlySpan<byte> data,
        out uint clientId1,
        out ushort checksum1,
        out uint clientId2,
        out ushort integrityCrc)
    {
        var key1 = data[0];
        var encodedKey2 = data[1];
        var key2 = (byte)(encodedKey2 ^ (byte)(key1 + 59));

        var maskedClientId1 = BinaryPrimitives.ReadUInt32BigEndian(data[2..]);
        var maskedChecksum1 = BinaryPrimitives.ReadUInt16BigEndian(data[6..]);
        var maskedClientId2 = BinaryPrimitives.ReadUInt32BigEndian(data[8..]);
        var maskedIntegrityChecksum = BinaryPrimitives.ReadUInt16BigEndian(data[12..]);

        //unmask ClientId1
        var clientId1Key = (byte)(key2 + 138);

        var mask32 = clientId1Key | ((uint)(clientId1Key + 1) << 8) | ((uint)(clientId1Key + 2) << 16) | ((uint)(clientId1Key + 3) << 24);

        clientId1 = maskedClientId1 ^ mask32;

        //unmask Checksum1 (CRC16 of clientId1 little-endian bytes)
        var checksum1Key = (byte)(key2 + 0x5E);
        var mask16 = (ushort)(checksum1Key | ((ushort)(checksum1Key + 1) << 8));
        checksum1 = (ushort)(maskedChecksum1 ^ mask16);

        //unmask ClientId2
        var clientId2Key = (byte)(key2 + 115);

        mask32 = clientId2Key | ((uint)(clientId2Key + 1) << 8) | ((uint)(clientId2Key + 2) << 16) | ((uint)(clientId2Key + 3) << 24);

        clientId2 = maskedClientId2 ^ mask32;

        //unmask IntegrityCrc (CRC16 of the first 12 encrypted bytes)
        var integrityCrcKey = (byte)(key2 + 165);
        mask16 = (ushort)(integrityCrcKey | ((ushort)(integrityCrcKey + 1) << 8));
        integrityCrc = (ushort)(maskedIntegrityChecksum ^ mask16);
    }

    /// <inheritdoc />
    public override LoginArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();

        //read the 14-byte payload (12 bytes for CRC input + 2 bytes for integrity checksum)
        var data = reader.ReadBytes(14);

        DecodeClientInfo(
            data,
            out var clientId1,
            out var checksum1,
            out var clientId2,
            out var integrityChecksum);

        return new LoginArgs
        {
            Name = name,
            Password = pw,
            ClientId1 = clientId1,
            Checksum1 = checksum1,
            ClientId2 = clientId2,
            IntegrityCrc = integrityChecksum
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LoginArgs args)
    {
        writer.WriteString8(args.Name);
        writer.WriteString8(args.Password);

        var key1 = Random.Shared.Next<byte>(0, byte.MaxValue);
        var key2 = Random.Shared.Next<byte>(0, byte.MaxValue);

        //mask ClientId1
        var clientId1Key = (byte)(key2 + 138);

        var mask32 = clientId1Key | ((uint)(clientId1Key + 1) << 8) | ((uint)(clientId1Key + 2) << 16) | ((uint)(clientId1Key + 3) << 24);

        var maskedClientId1 = args.ClientId1 ^ mask32;

        //mask Checksum1 (CRC16 of clientId1 little-endian bytes)
        var checksum1 = Crc.Generate16(BitConverter.GetBytes(args.ClientId1));
        var checksum1Key = (byte)(key2 + 0x5E);
        var mask16 = (ushort)(checksum1Key | ((ushort)(checksum1Key + 1) << 8));
        var maskedChecksum1 = (ushort)(checksum1 ^ mask16);

        //mask ClientId2
        var clientId2Key = (byte)(key2 + 115);

        mask32 = clientId2Key | ((uint)(clientId2Key + 1) << 8) | ((uint)(clientId2Key + 2) << 16) | ((uint)(clientId2Key + 3) << 24);

        var maskedClientId2 = args.ClientId2 ^ mask32;

        //build the 12-byte payload to compute integrity CRC
        var encodedKey2 = (byte)(key2 ^ (key1 + 59));

        Span<byte> encryptedPayload = stackalloc byte[12];

        encryptedPayload[0] = key1;
        encryptedPayload[1] = encodedKey2;
        BinaryPrimitives.WriteUInt32BigEndian(encryptedPayload[2..], maskedClientId1);
        BinaryPrimitives.WriteUInt16BigEndian(encryptedPayload[6..], maskedChecksum1);
        BinaryPrimitives.WriteUInt32BigEndian(encryptedPayload[8..], maskedClientId2);

        //write payload to the actual output
        writer.WriteBytes(encryptedPayload);

        //compute integrity CRC over the 12 masked bytes
        var integrityCrc = Crc.Generate16(encryptedPayload);
        var integrityCrcKey = (byte)(key2 + 165);
        mask16 = (ushort)(integrityCrcKey | ((ushort)(integrityCrcKey + 1) << 8));
        var maskedIntegrityChecksum = (ushort)(integrityCrc ^ mask16);

        writer.WriteUInt16(maskedIntegrityChecksum);

        //trailing bytes
        writer.WriteByte(0x01);
        writer.WriteByte(0x00);
    }
}