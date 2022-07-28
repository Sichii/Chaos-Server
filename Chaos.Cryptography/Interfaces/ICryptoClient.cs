namespace Chaos.Cryptography.Interfaces;

public interface ICryptoClient
{
    byte[] Key { get; }
    byte Seed { get; }

    /// <summary>
    ///     Decrypts a packet that's been sent from a client.
    /// </summary>
    void Decrypt(ref Span<byte> buffer, byte opCode, byte sequence);

    /// <summary>
    ///     Decrypts the dialog header of a packet sent from a client.
    /// </summary>
    void DecryptDialog(ref Span<byte> buffer);

    /// <summary>
    ///     Encrypts a packet that's being sent to a client.
    /// </summary>
    void Encrypt(ref Span<byte> buffer, byte opCode, byte sequence);

    byte[] GenerateKey(ushort a, byte b);
    byte[] GenerateKeySalts(string seed);

    /// <summary>
    ///     Which type of encryption, if any, should be used with the given opcode on a packet sent from the client.
    /// </summary>
    EncryptionType GetClientEncryptionType(byte opCode);

    string GetMd5Hash(string value);

    /// <summary>
    ///     Which type of encryption, if any, should be used with the given opcode on a packet sent from the server.
    /// </summary>
    EncryptionType ServerEncryptionType(byte opCode);

    /// <summary>
    ///     Whether or not a packet with the given opcode sent from the client should be encrypted.
    /// </summary>
    bool ShouldBeEncrypted(byte opCode);

    /// <summary>
    ///     Whether or not a packet with the given opcode sent from the server should be encrypted.
    /// </summary>
    bool ShouldEncrypt(byte opCode);
}