#region
using System.Buffers;
using Chaos.Cryptography.Abstractions;
using Chaos.Cryptography.Abstractions.Definitions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockCrypto : ICrypto
{
    public int IsServerEncryptedCallCount { get; private set; }
    public bool IsServerEncryptedResult { get; set; }
    public int ServerDecryptCallCount { get; private set; }
    public int ServerEncryptCallCount { get; private set; }

    public byte[] Key { get; } = [];
    public byte Seed { get; } = 0;

    public void ClientDecrypt(ref Span<byte> buffer, byte opCode, byte sequence) { }

    public void ClientEncrypt(ref Span<byte> buffer, byte opCode, byte sequence) { }

    public void DecryptDialog(ref Span<byte> buffer) { }

    public void EncryptDialog(ref Span<byte> buffer) { }

    public void GenerateEncryptionParameters() { }

    public byte[] GenerateKey(ushort a, byte b) => [];

    public byte[] GenerateKeySalts(string seed) => [];

    public EncryptionType GetClientEncryptionType(byte opCode) => EncryptionType.None;

    public string GetMd5Hash(string value) => string.Empty;

    public EncryptionType GetServerEncryptionType(byte opCode) => EncryptionType.None;

    public bool IsClientEncrypted(byte opCode) => false;

    public bool IsServerEncrypted(byte opCode)
    {
        IsServerEncryptedCallCount++;

        return IsServerEncryptedResult;
    }

    public void ServerDecrypt(ref Span<byte> buffer, byte opCode, byte sequence) => ServerDecryptCallCount++;

    public void ServerEncrypt(
        ref IMemoryOwner<byte> memoryOwner,
        ref int length,
        byte opCode,
        byte sequence)
        => ServerEncryptCallCount++;

    public static MockCrypto Create() => new();
}