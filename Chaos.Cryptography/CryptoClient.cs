using System.Security.Cryptography;
using System.Text;
using Chaos.Cryptography.Extensions;
using Chaos.Cryptography.Interfaces;

namespace Chaos.Cryptography;

public class CryptoClient : ICryptoClient
{
    private readonly IReadOnlyList<byte> KeySalts;
    public byte[] Key { get; }
    public byte Seed { get; }
    private IReadOnlyList<byte> Salts => Tables.SALT_TABLE[Seed];

    public CryptoClient()
        : this(0, Encoding.ASCII.GetBytes("UrkcnItnI"), string.Empty) { }

    public CryptoClient(byte seed, byte[] key, string keySaltSeed)
    {
        Seed = seed;
        Key = key;
        KeySalts = string.IsNullOrEmpty(keySaltSeed) ? new byte[1024] : GenerateKeySalts(keySaltSeed);
    }

    public byte[] GenerateKey(ushort a, byte b)
    {
        var key = new byte[9];

        for (var i = 0; i < 9; i++)
            key[i] = KeySalts[(i * (9 * i + b * b) + a) % KeySalts.Count];

        return key;
    }

    public byte[] GenerateKeySalts(string seed)
    {
        var saltTable = GetMd5Hash(GetMd5Hash(seed));

        for (var i = 0; i < 31; i++)
            saltTable += GetMd5Hash(saltTable);

        return Encoding.ASCII.GetBytes(saltTable);
    }

    #region Client Encryption
    /// <summary>
    ///     Whether or not a packet with the given opcode sent from the client should be encrypted.
    /// </summary>
    public bool ShouldBeEncrypted(byte opCode) => GetClientEncryptionType(opCode) != EncryptionType.None;

    /// <summary>
    ///     Which type of encryption, if any, should be used with the given opcode on a packet sent from the client.
    /// </summary>
    public EncryptionType GetClientEncryptionType(byte opCode) =>
        opCode switch
        {
            0   => EncryptionType.None,
            16  => EncryptionType.None,
            72  => EncryptionType.None,
            2   => EncryptionType.Normal,
            3   => EncryptionType.Normal,
            4   => EncryptionType.Normal,
            11  => EncryptionType.Normal,
            38  => EncryptionType.Normal,
            45  => EncryptionType.Normal,
            58  => EncryptionType.Normal,
            66  => EncryptionType.Normal,
            67  => EncryptionType.Normal,
            75  => EncryptionType.Normal,
            87  => EncryptionType.Normal,
            98  => EncryptionType.Normal,
            104 => EncryptionType.Normal,
            113 => EncryptionType.Normal,
            115 => EncryptionType.Normal,
            123 => EncryptionType.Normal,
            _   => EncryptionType.MD5
        };

    /// <summary>
    ///     Encrypts a packet that's being sent to a client.
    /// </summary>
    public void Encrypt(ref Span<byte> buffer, byte opCode, byte sequence)
    {
        IReadOnlyList<byte> thisKey;
        var a = (ushort)Random.Shared.Next(256, ushort.MaxValue);
        var b = (byte)Random.Shared.Next(100, byte.MaxValue);
        var type = ServerEncryptionType(opCode);

        switch (type)
        {
            case EncryptionType.Normal:
                thisKey = Key;

                break;
            case EncryptionType.MD5:
                thisKey = GenerateKey(a, b);

                break;
            default:
                return;
        }

        for (var i = 0; i < buffer.Length; i++)
        {
            var index = (byte)(i / Key.Length);
            buffer[i] ^= (byte)(Salts[index] ^ thisKey[i % thisKey.Count]);

            if (index != sequence)
                buffer[i] ^= Salts[sequence];
        }

        var newBuffer = new Span<byte>(new byte[buffer.Length + 3]);
        buffer.CopyTo(newBuffer);

        newBuffer[^3] = (byte)((byte)a ^ 116);
        newBuffer[^2] = (byte)(b ^ 36U);
        newBuffer[^1] = (byte)((byte)(a >> 8) ^ 100);

        //overwrite ref
        buffer = newBuffer;
    }

    /// <summary>
    ///     Decrypts a packet that's been sent from a client.
    /// </summary>
    public void Decrypt(ref Span<byte> buffer, byte opCode, byte sequence)
    {
        var length = buffer.Length - 7;
        IReadOnlyList<byte> thisKey;
        var a = (ushort)(((buffer[length + 6] << 8) | buffer[length + 4]) ^ 29808);
        var b = (byte)(buffer[length + 5] ^ 35);
        var type = GetClientEncryptionType(opCode);

        switch (type)
        {
            case EncryptionType.Normal:
                length--;
                thisKey = Key;

                break;
            case EncryptionType.MD5:
                length -= 2;
                thisKey = GenerateKey(a, b);

                break;
            default:
                return;
        }

        for (var i = 0; i < length; ++i)
        {
            var index = (byte)(i / Key.Length);
            buffer[i] ^= (byte)(Salts[index] ^ thisKey[i % thisKey.Count]);

            if (index != sequence)
                buffer[i] ^= Salts[sequence];
        }

        var slice = buffer[..length];
        //overwrite ref
        buffer = slice;

        if (opCode is 57 or 58)
            DecryptDialog(ref buffer);
    }

    /// <summary>
    ///     Decrypts the dialog header of a packet sent from a client.
    /// </summary>
    public void DecryptDialog(ref Span<byte> buffer)
    {
        var num1 = (byte)(buffer[1] ^ (uint)(byte)(buffer[0] - 45U));
        var num2 = (byte)(num1 + 114U);
        var num3 = (byte)(num1 + 40U);

        buffer[2] ^= num2;
        buffer[3] ^= (byte)(num2 + 1);
        var num4 = (buffer[2] << 8) | buffer[3];

        for (var index = 0; index < num4; index++)
            buffer[4 + index] ^= (byte)(num3 + index);

        var slice = buffer[6..];

        //overwrite ref
        buffer = slice;
    }
    #endregion

    #region Server Encryption
    /// <summary>
    ///     Whether or not a packet with the given opcode sent from the server should be encrypted.
    /// </summary>
    public bool ShouldEncrypt(byte opCode) => ServerEncryptionType(opCode) != EncryptionType.None;

    /// <summary>
    ///     Which type of encryption, if any, should be used with the given opcode on a packet sent from the server.
    /// </summary>
    public EncryptionType ServerEncryptionType(byte opCode) =>
        opCode switch
        {
            0   => EncryptionType.None,
            3   => EncryptionType.None,
            64  => EncryptionType.None,
            126 => EncryptionType.None,
            1   => EncryptionType.Normal,
            2   => EncryptionType.Normal,
            10  => EncryptionType.Normal,
            86  => EncryptionType.Normal,
            96  => EncryptionType.Normal,
            98  => EncryptionType.Normal,
            102 => EncryptionType.Normal,
            111 => EncryptionType.Normal,
            _   => EncryptionType.MD5
        };
    #endregion

    #region Utility
    public string GetMd5Hash(string value) =>
        BitConverter.ToString(
                        MD5.Create()
                           .ComputeHash(Encoding.ASCII.GetBytes(value)))
                    .Replace("-", string.Empty)
                    .ToLower();
    
    public ushort GenerateFieldNodeChecksum(ushort mapId, byte x, byte y, string text)
    {
        using var data = new MemoryStream();
        using var writer = new BinaryWriter(data);

        writer.Write(Encoding.Unicode.GetBytes(text));
        writer.Write(mapId);
        writer.Write(x);
        writer.Write(y);

        writer.Flush();

        return data.ToArray()
                   .Generate16();
    }
    #endregion
}