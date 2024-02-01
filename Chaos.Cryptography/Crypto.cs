using System.Security.Cryptography;
using System.Text;
using Chaos.Cryptography.Abstractions;
using Chaos.Cryptography.Abstractions.Definitions;

namespace Chaos.Cryptography;

/// <summary>
///     Provides encryption and decryption for spans using opcodes
/// </summary>
public sealed class Crypto : ICrypto
{
    private readonly IReadOnlyList<byte> KeySalts;

    /// <inheritdoc />
    public byte[] Key { get; }

    /// <inheritdoc />
    public byte Seed { get; }

    private IReadOnlyList<byte> Salts => Tables.SALT_TABLE[Seed];

    /// <summary>
    ///     Creates a new instance of the Crypto class
    /// </summary>
    public Crypto()
        : this(0, "UrkcnItnI"u8.ToArray(), string.Empty) { }

    /// <summary>
    ///     Creates a new instance of the Crypto class
    /// </summary>
    /// <param name="seed">
    ///     The seed used to salt to key
    /// </param>
    /// <param name="key">
    ///     The encryption key
    /// </param>
    /// <param name="keySaltSeed">
    ///     The seed used to generate key salts
    /// </param>
    public Crypto(byte seed, byte[] key, string? keySaltSeed = null)
    {
        Seed = seed;
        Key = key;
        KeySalts = string.IsNullOrEmpty(keySaltSeed) ? new byte[1024] : GenerateKeySalts(keySaltSeed);
    }

    /// <summary>
    ///     Creates a new instance of the Crypto class
    /// </summary>
    public Crypto(byte seed, string keySaltSeed)
    {
        Seed = seed;
        KeySalts = GenerateKeySalts(keySaltSeed);

        var a = (ushort)Random.Shared.Next(256, ushort.MaxValue);
        var b = (byte)Random.Shared.Next(100, byte.MaxValue);
        Key = GenerateKey(a, b);
    }

    /// <inheritdoc />
    public byte[] GenerateKey(ushort a, byte b)
    {
        var key = new byte[9];

        for (var i = 0; i < 9; i++)
            key[i] = KeySalts[(i * (9 * i + b * b) + a) % KeySalts.Count];

        return key;
    }

    /// <inheritdoc />
    public byte[] GenerateKeySalts(string seed)
    {
        var saltTable = GetMd5Hash(GetMd5Hash(seed));

        for (var i = 0; i < 31; i++)
            saltTable += GetMd5Hash(saltTable);

        return Encoding.ASCII.GetBytes(saltTable);
    }

    #region Utility
    /// <inheritdoc />
    public string GetMd5Hash(string value)
        => BitConverter.ToString(MD5.HashData(Encoding.ASCII.GetBytes(value)))
                       .Replace("-", string.Empty)
                       .ToLower();
    #endregion

    #region Client Encryption
    /// <summary>
    ///     Whether or not a packet with the given opcode sent from the client should be encrypted.
    /// </summary>
    public bool IsClientEncrypted(byte opCode) => GetClientEncryptionType(opCode) != EncryptionType.None;

    /// <summary>
    ///     Which type of encryption, if any, should be used with the given opcode on a packet sent from a client.
    /// </summary>
    public EncryptionType GetClientEncryptionType(byte opCode)
        => opCode switch
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
    ///     Decrypts a packet that's been sent to a client
    /// </summary>
    public void ClientDecrypt(ref Span<byte> buffer, byte opCode, byte sequence)
    {
        byte[] thisKey;
        var resultLength = buffer.Length - 3;
        var a = (ushort)(((buffer[resultLength + 2] << 8) | buffer[resultLength]) ^ 25716);
        var b = (byte)(buffer[resultLength + 1] ^ 36U);
        var type = GetServerEncryptionType(opCode);

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

        for (var index1 = 0; index1 < resultLength; ++index1)
        {
            var index2 = index1 / Key.Length % 256;
            buffer[index1] ^= (byte)(Salts[index2] ^ (uint)thisKey[index1 % thisKey.Length]);

            if (index2 != sequence)
                buffer[index1] ^= Salts[sequence];
        }

        buffer = buffer[..resultLength];
    }

    /// <summary>
    ///     Encrypts a packet that's being sent from a client
    /// </summary>
    public void ClientEncrypt(ref Span<byte> buffer, byte opcode, byte sequence)
    {
        if (opcode is 57 or 58)
            EncryptDialog(ref buffer);

        byte[] thisKey;
        var a = (ushort)Random.Shared.Next(256, ushort.MaxValue);
        var b = (byte)Random.Shared.Next(100, byte.MaxValue);
        var type = GetClientEncryptionType(opcode);
        var resultLength = buffer.Length + 8;
        var position = buffer.Length + 1;
        Span<byte> newBuffer;

        switch (type)
        {
            case EncryptionType.Normal:
                thisKey = Key;
                newBuffer = new Span<byte>(new byte[resultLength]);
                buffer.CopyTo(newBuffer);

                break;
            case EncryptionType.MD5:
                thisKey = GenerateKey(a, b);
                newBuffer = new Span<byte>(new byte[resultLength + 1]);
                buffer.CopyTo(newBuffer);

                newBuffer[position++] = opcode;

                break;
            default:
                return;
        }

        for (var i = 0; i < position; ++i)
        {
            var saltIndex = i / Key.Length % 256;

            newBuffer[i] ^= (byte)(Salts[saltIndex] ^ (uint)thisKey[i % thisKey.Length]);

            if (saltIndex != sequence)
                newBuffer[i] ^= Salts[sequence];
        }

        Span<byte> bytesToHash = stackalloc byte[position + 2];
        bytesToHash[0] = opcode;
        bytesToHash[1] = sequence;

        newBuffer[..position]
            .CopyTo(bytesToHash[2..]);

        var hash = MD5.HashData(bytesToHash);

        newBuffer[position++] = hash[13];
        newBuffer[position++] = hash[3];
        newBuffer[position++] = hash[11];
        newBuffer[position++] = hash[7];

        newBuffer[position++] = (byte)((a % 256) ^ 112);
        newBuffer[position++] = (byte)(b ^ 35);
        newBuffer[position] = (byte)(((a >> 8) % 256) ^ 116);

        buffer = newBuffer;
    }

    /// <summary>
    ///     Encrypts a packet that's being sent from a server
    /// </summary>
    public void ServerEncrypt(ref Span<byte> buffer, byte opCode, byte sequence)
    {
        IReadOnlyList<byte> thisKey;
        var a = (ushort)Random.Shared.Next(256, ushort.MaxValue);
        var b = (byte)Random.Shared.Next(100, byte.MaxValue);
        var type = GetServerEncryptionType(opCode);

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
            var i2 = (byte)(i / Key.Length);
            buffer[i] ^= (byte)(Salts[i2] ^ thisKey[i % thisKey.Count]);

            if (i2 != sequence)
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
    ///     Decrypts a packet that's been sent to a server
    /// </summary>
    public void ServerDecrypt(ref Span<byte> buffer, byte opCode, byte sequence)
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
            var i2 = (byte)(i / Key.Length);
            buffer[i] ^= (byte)(Salts[i2] ^ thisKey[i % thisKey.Count]);

            if (i2 != sequence)
                buffer[i] ^= Salts[sequence];
        }

        buffer = buffer[..length];

        if (opCode is 57 or 58)
            DecryptDialog(ref buffer);
    }

    /// <summary>
    ///     Encrypts the dialog of a packet sent from a client
    /// </summary>
    public void EncryptDialog(ref Span<byte> buffer)
    {
        var newBuffer = new Span<byte>(new byte[buffer.Length + 6]);
        buffer.CopyTo(newBuffer);
        buffer.CopyTo(newBuffer[6..]);

        var checksum = Crc.Generate16(newBuffer[6..]);
        newBuffer[0] = (byte)Random.Shared.Next();
        newBuffer[1] = (byte)Random.Shared.Next();
        newBuffer[2] = (byte)((newBuffer.Length - 4) / 256);
        newBuffer[3] = (byte)((newBuffer.Length - 4) % 256);
        newBuffer[4] = (byte)(checksum / 256);
        newBuffer[5] = (byte)(checksum % 256);

        var num1 = (newBuffer[2] << 8) | newBuffer[3];
        var num2 = (byte)(newBuffer[1] ^ (uint)(byte)(newBuffer[0] - 45U));
        var num3 = (byte)(num2 + 114U);
        var num4 = (byte)(num2 + 40U);

        newBuffer[2] ^= num3;
        newBuffer[3] ^= (byte)((num3 + 1) % 256);

        for (var index = 0; index < num1; ++index)
            newBuffer[4 + index] ^= (byte)((num4 + index) % 256);

        buffer = newBuffer;
    }

    /// <summary>
    ///     Decrypts the dialog of a packet sent from a client.
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
    public bool IsServerEncrypted(byte opCode) => GetServerEncryptionType(opCode) != EncryptionType.None;

    /// <summary>
    ///     Which type of encryption, if any, should be used with the given opcode on a packet sent from the server.
    /// </summary>
    public EncryptionType GetServerEncryptionType(byte opCode)
        => opCode switch
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
}