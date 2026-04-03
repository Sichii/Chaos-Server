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
    ///     Decodes both client ids and their checksums from the encrypted values
    /// </summary>
    public static void DecodeClientInfo(
        byte key1,
        byte encodedKey2,
        uint encryptedClientId1,
        ushort encryptedChecksum1,
        uint encryptedClientId2,
        ushort encryptedChecksum2,
        out uint clientId1,
        out ushort checksum1,
        out uint clientId2,
        out ushort checksum2)
    {
        var key2 = (byte)(encodedKey2 ^ (byte)(key1 + 59));

        // Decode ClientId1
        var clientId1Key = (byte)(key2 + 138);

        var mask32 = clientId1Key | ((uint)(clientId1Key + 1) << 8) | ((uint)(clientId1Key + 2) << 16) | ((uint)(clientId1Key + 3) << 24);

        clientId1 = encryptedClientId1 ^ mask32;

        var checksum1Key = (byte)(key2 + 0x5E);
        var mask16 = (ushort)(checksum1Key | ((ushort)(checksum1Key + 1) << 8));
        checksum1 = (ushort)(encryptedChecksum1 ^ mask16);

        // Decode ClientId2
        var clientId2Key = (byte)(key2 + 115);

        mask32 = clientId2Key | ((uint)(clientId2Key + 1) << 8) | ((uint)(clientId2Key + 2) << 16) | ((uint)(clientId2Key + 3) << 24);

        clientId2 = encryptedClientId2 ^ mask32;

        var checksum2Key = (byte)(key2 + 165);
        mask16 = (ushort)(checksum2Key | ((ushort)(checksum2Key + 1) << 8));
        checksum2 = (ushort)(encryptedChecksum2 ^ mask16);
    }

    /// <inheritdoc />
    public override LoginArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();
        var key1 = reader.ReadByte();
        var encodedKey2 = reader.ReadByte();
        var encryptedClientId1 = reader.ReadUInt32();
        var encryptedChecksum1 = reader.ReadUInt16();
        var encryptedClientId2 = reader.ReadUInt32();
        var encryptedChecksum2 = reader.ReadUInt16();

        DecodeClientInfo(
            key1,
            encodedKey2,
            encryptedClientId1,
            encryptedChecksum1,
            encryptedClientId2,
            encryptedChecksum2,
            out var clientId1,
            out var checksum1,
            out var clientId2,
            out var checksum2);

        return new LoginArgs
        {
            Name = name,
            Password = pw,
            ClientId1 = clientId1,
            Checksum1 = checksum1,
            ClientId2 = clientId2,
            Checksum2 = checksum2
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, LoginArgs args)
    {
        writer.WriteString8(args.Name);
        writer.WriteString8(args.Password);

        var key1 = Random.Shared.Next<byte>(0, byte.MaxValue);
        var key2 = Random.Shared.Next<byte>(0, byte.MaxValue);

        // Encrypt ClientId1
        var clientId1Key = (byte)(key2 + 138);

        var mask32 = clientId1Key | ((uint)(clientId1Key + 1) << 8) | ((uint)(clientId1Key + 2) << 16) | ((uint)(clientId1Key + 3) << 24);

        var encryptedClientId1 = args.ClientId1 ^ mask32;

        var checksum1 = Crc.Generate16(BitConverter.GetBytes(args.ClientId1));
        var checksum1Key = (byte)(key2 + 0x5E);
        var mask16 = (ushort)(checksum1Key | ((ushort)(checksum1Key + 1) << 8));
        var encryptedChecksum1 = (ushort)(checksum1 ^ mask16);

        writer.WriteByte(key1);
        writer.WriteByte((byte)(key2 ^ (key1 + 59)));
        writer.WriteUInt32(encryptedClientId1);
        writer.WriteUInt16(encryptedChecksum1);

        // Encrypt ClientId2
        var clientId2Key = (byte)(key2 + 115);

        mask32 = clientId2Key | ((uint)(clientId2Key + 1) << 8) | ((uint)(clientId2Key + 2) << 16) | ((uint)(clientId2Key + 3) << 24);

        var encryptedClientId2 = args.ClientId2 ^ mask32;

        var checksum2 = Crc.Generate16(BitConverter.GetBytes(args.ClientId2));
        var checksum2Key = (byte)(key2 + 165);
        mask16 = (ushort)(checksum2Key | ((ushort)(checksum2Key + 1) << 8));
        var encryptedChecksum2 = (ushort)(checksum2 ^ mask16);

        writer.WriteUInt32(encryptedClientId2);
        writer.WriteUInt16(encryptedChecksum2);
        writer.WriteUInt16(256);
    }
}