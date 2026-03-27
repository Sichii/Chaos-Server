#region
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
    ///     Decodes the client id and checksum from the encrypted values
    /// </summary>
    public static void DecodeClientInfo(
        byte key1,
        byte encodedKey2,
        uint encryptedClientId,
        ushort encryptedChecksum,
        out uint originalClientId,
        out ushort originalChecksum)
    {
        var key2 = (byte)(encodedKey2 ^ (byte)(key1 + 59));

        var clientIdKey = (byte)(key2 + 138);
        var mask32 = clientIdKey | ((uint)(clientIdKey + 1) << 8) | ((uint)(clientIdKey + 2) << 16) | ((uint)(clientIdKey + 3) << 24);

        originalClientId = encryptedClientId ^ mask32;

        var checksumKey = (byte)(key2 + 0x5E);
        var mask16 = (ushort)(checksumKey | ((ushort)(checksumKey + 1) << 8));

        originalChecksum = (ushort)(encryptedChecksum ^ mask16);
    }

    /// <inheritdoc />
    public override LoginArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();
        var key1 = reader.ReadByte();
        var key2 = reader.ReadByte();
        var encryptedClientId = reader.ReadUInt32();
        var encryptedChecksum = reader.ReadUInt16();

        DecodeClientInfo(
            key1,
            key2,
            encryptedClientId,
            encryptedChecksum,
            out var clientId,
            out var checksum);

        return new LoginArgs
        {
            Name = name,
            Password = pw,
            ClientId1 = clientId,
            ClientId2 = checksum
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LoginArgs args)
    {
        writer.WriteString8(args.Name);
        writer.WriteString8(args.Password);

        //generate random keys
        var key1 = Random.Shared.Next<byte>(0, byte.MaxValue);
        var key2 = Random.Shared.Next<byte>(0, byte.MaxValue);

        //encrypt client id
        var clientIdKey = (byte)(key2 + 138);

        var mask32 = clientIdKey | ((uint)(clientIdKey + 1) << 8) | ((uint)(clientIdKey + 2) << 16) | ((uint)(clientIdKey + 3) << 24);

        var encryptedClientId = args.ClientId1 ^ mask32;

        //encrypt checksum
        var checksumKey = (byte)(key2 + 0x5E);
        var mask16 = (ushort)(checksumKey | ((ushort)(checksumKey + 1) << 8));

        var encryptedChecksum = (ushort)(args.ClientId2 ^ mask16);

        writer.WriteByte(key1);
        writer.WriteByte((byte)(key2 ^ (key1 + 59)));
        writer.WriteUInt32(encryptedClientId);
        writer.WriteUInt16(encryptedChecksum);

        //encrypt and write random id B
        var randomIdB = Random.Shared.Next<uint>(0, 0xFFFF);
        var randomIdBKey = (byte)(key2 + 115);

        var randomIdBMask = randomIdBKey
                            | ((uint)(randomIdBKey + 1) << 8)
                            | ((uint)(randomIdBKey + 2) << 16)
                            | ((uint)(randomIdBKey + 3) << 24);

        writer.WriteUInt32(randomIdB ^ randomIdBMask);

        //compute and encrypt key checksum over all key data written so far
        var buffer = writer.ToSpan();
        var keyBuffer = buffer[(args.Name.Length + args.Password.Length + 2)..];
        var keyChecksum = Crc.Generate16(keyBuffer);
        var keyChecksumKey = (byte)(key2 + 165);
        keyChecksum ^= (ushort)(keyChecksumKey | ((ushort)(keyChecksumKey + 1) << 8));

        writer.WriteUInt16(keyChecksum);
        writer.WriteUInt16(256);
    }
}