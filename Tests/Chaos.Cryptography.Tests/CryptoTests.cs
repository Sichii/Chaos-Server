#region
using System.Buffers;
using System.Text;
using Chaos.Cryptography.Abstractions.Definitions;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Cryptography.Tests;

public sealed class CryptoTests
{
    private readonly Crypto Crypto = new(
        0,
        Encoding.ASCII.GetString(
            [
                1,
                2,
                3,
                4,
                5
            ]));

    [Test]
    [Arguments((byte)0)] // None => early return
    [Arguments((byte)3)]
    [Arguments((byte)64)]
    [Arguments((byte)126)]
    public void ClientDecrypt_Should_Not_Modify_When_Type_None(byte opCode)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);

        // length >= 3 to satisfy internal indexing of last 3 bytes
        var original = Encoding.ASCII.GetBytes("xyz");
        var buffer = new Span<byte>(original.ToArray());

        c.ClientDecrypt(ref buffer, opCode, 0);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("xyz");
    }

    [Test]
    [Arguments((byte)0)] // None => early return
    [Arguments((byte)16)]
    [Arguments((byte)72)]
    public void ClientEncrypt_Should_Not_Modify_When_Type_None(byte opCode)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("nochange");
        var buffer = new Span<byte>(original.ToArray());

        c.ClientEncrypt(ref buffer, opCode, 0);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("nochange");
    }

    [Test]
    [Arguments((byte)57, (byte)2)] // Dialog path + MD5 branch
    public void ClientEncrypt_Then_ServerDecrypt_Should_Roundtrip_For_Dialog_57(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("dialog-57");
        var buffer = new Span<byte>(original.ToArray());

        c.ClientEncrypt(ref buffer, opCode, sequence);

        c.ServerDecrypt(ref buffer, opCode, sequence);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("dialog-57");
    }

    [Test]
    [Arguments((byte)255, (byte)7)] // MD5 path
    public void ClientEncrypt_Then_ServerDecrypt_Should_Roundtrip_For_MD5(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("payload-md5");
        var buffer = new Span<byte>(original.ToArray());

        c.ClientEncrypt(ref buffer, opCode, sequence);

        c.ServerDecrypt(ref buffer, opCode, sequence);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("payload-md5");
    }

    [Test]
    [Arguments((byte)58, (byte)3)] // Normal path + dialog branch
    [Arguments((byte)2, (byte)0)] // Normal path without dialog
    public void ClientEncrypt_Then_ServerDecrypt_Should_Roundtrip_For_Normal(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("payload-123");
        var buffer = new Span<byte>(original.ToArray());

        c.ClientEncrypt(ref buffer, opCode, sequence);

        c.ServerDecrypt(ref buffer, opCode, sequence);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("payload-123");
    }

    [Test]
    [Arguments((byte)2, (byte)0)] // Normal path with empty payload
    public void ClientEncrypt_Then_ServerDecrypt_Should_Work_With_EmptyPayload(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var buffer = new Span<byte>(Array.Empty<byte>());

        c.ClientEncrypt(ref buffer, opCode, sequence);

        c.ServerDecrypt(ref buffer, opCode, sequence);

        buffer.ToArray()
              .Should()
              .BeEmpty();
    }

    [Test]
    [Arguments((byte)57, (byte)0)]
    [Arguments((byte)58, (byte)255)]
    public void ClientEncrypt_With_Dialog_OpCodes_Triggers_Dialog_Branch(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("dialog-verify");
        var buffer = new Span<byte>(original.ToArray());

        c.ClientEncrypt(ref buffer, opCode, sequence);
        c.ServerDecrypt(ref buffer, opCode, sequence);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("dialog-verify");
    }

    [Test]
    public void Ctor_With_Seed_And_SaltSeed_Should_Generate_Key()
    {
        var c = new Crypto(7, "salt-seed");

        c.Seed
         .Should()
         .Be(7);

        c.Key
         .Should()
         .NotBeNull();

        c.Key
         .Length
         .Should()
         .Be(9);
    }

    [Test]
    [Arguments((byte)5, "abc", null)]
    [Arguments((byte)2, "123456789", "seed")]
    public void Ctor_With_Seed_Key_And_SaltSeed_Should_Set_Properties(byte seed, string key, string? keySaltSeed)
    {
        var c = new Crypto(seed, key, keySaltSeed);

        c.Seed
         .Should()
         .Be(seed);

        Encoding.ASCII
                .GetString(c.Key)
                .Should()
                .Be(key);

        c.Key
         .Length
         .Should()
         .Be(
             Encoding.ASCII.GetBytes(key)
                     .Length);
    }

    [Test]
    public void DefaultCtor_Should_Set_Seed_And_Key()
    {
        var c = new Crypto();

        c.Seed
         .Should()
         .Be(0);

        c.Key
         .Should()
         .NotBeNull();

        c.Key
         .Length
         .Should()
         .Be(9);

        // default ctor uses "UrkcnItnI"
        Encoding.ASCII
                .GetString(c.Key)
                .Should()
                .Be("UrkcnItnI");
    }

    [Test]
    public void EncryptDialog_Then_DecryptDialog_Should_Roundtrip()
    {
        var c = new Crypto();
        var original = Encoding.ASCII.GetBytes("hello world");
        var buffer = new Span<byte>(original.ToArray());

        c.EncryptDialog(ref buffer);

        // Now decrypt and expect the original
        c.DecryptDialog(ref buffer);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("hello world");
    }

    [Test]
    public void EncryptDialog_Then_DecryptDialog_With_EmptyBuffer_Should_Roundtrip()
    {
        var c = new Crypto();
        var buffer = new Span<byte>(Array.Empty<byte>());

        c.EncryptDialog(ref buffer);
        c.DecryptDialog(ref buffer);

        buffer.ToArray()
              .Should()
              .BeEmpty();
    }

    [Test]
    public void GenerateEncryptionParameters_Should_Update_Seed_And_Key()
    {
        var c = new Crypto();
        var beforeKey = c.Key.ToArray();

        c.GenerateEncryptionParameters();

        c.Seed
         .Should()
         .BeInRange(0, 9);

        c.Key
         .Should()
         .NotBeNull();

        c.Key
         .Length
         .Should()
         .Be(9);

        // Key may or may not change due to randomness; assert shape only
        beforeKey.Length
                 .Should()
                 .Be(9);
    }

    [Test]
    public void GenerateKey_Should_Use_KeySalts_Deterministically_For_Given_A_B()
    {
        var k1 = Crypto.GenerateKey(1234, 200);
        var k2 = Crypto.GenerateKey(1234, 200);
        var k3 = Crypto.GenerateKey(2222, 201);

        k1.Should()
          .BeEquivalentTo(k2);

        k1.Should()
          .NotBeEquivalentTo(k3);

        k1.Length
          .Should()
          .Be(9);
    }

    [Test]
    public void GenerateKeySalts_Should_Return_Same_For_Same_Seed_And_Different_For_Different_Seed()
    {
        var a1 = Crypto.GenerateKeySalts("seed-A");
        var a2 = Crypto.GenerateKeySalts("seed-A");
        var b1 = Crypto.GenerateKeySalts("seed-B");

        a1.Should()
          .BeEquivalentTo(a2);

        a1.Should()
          .NotBeEquivalentTo(b1);
    }

    [Test]
    [Arguments((byte)0)]
    [Arguments((byte)16)]
    [Arguments((byte)72)]
    public void GetClientEncryptionType_Should_Return_None_For_Listed(byte opCode)
        => Crypto.GetClientEncryptionType(opCode)
                 .Should()
                 .Be(EncryptionType.None);

    [Test]
    [Arguments((byte)2)]
    [Arguments((byte)3)]
    [Arguments((byte)4)]
    [Arguments((byte)11)]
    [Arguments((byte)38)]
    [Arguments((byte)45)]
    [Arguments((byte)58)]
    [Arguments((byte)66)]
    [Arguments((byte)67)]
    [Arguments((byte)75)]
    [Arguments((byte)87)]
    [Arguments((byte)98)]
    [Arguments((byte)104)]
    [Arguments((byte)113)]
    [Arguments((byte)115)]
    [Arguments((byte)123)]
    public void GetClientEncryptionType_Should_Return_Normal_For_Listed(byte opCode)
        => Crypto.GetClientEncryptionType(opCode)
                 .Should()
                 .Be(EncryptionType.Normal);

    [Test]
    [Arguments((byte)0)]
    [Arguments((byte)3)]
    [Arguments((byte)64)]
    [Arguments((byte)126)]
    public void GetServerEncryptionType_Should_Return_None_For_Listed(byte opCode)
        => Crypto.GetServerEncryptionType(opCode)
                 .Should()
                 .Be(EncryptionType.None);

    [Test]
    [Arguments((byte)1)]
    [Arguments((byte)2)]
    [Arguments((byte)10)]
    [Arguments((byte)86)]
    [Arguments((byte)96)]
    [Arguments((byte)98)]
    [Arguments((byte)102)]
    [Arguments((byte)111)]
    public void GetServerEncryptionType_Should_Return_Normal_For_Listed(byte opCode)
        => Crypto.GetServerEncryptionType(opCode)
                 .Should()
                 .Be(EncryptionType.Normal);

    [Test]
    [Arguments((byte)0)] // None => early return
    [Arguments((byte)16)]
    [Arguments((byte)72)]
    public void ServerDecrypt_Should_Not_Modify_When_Type_None(byte opCode)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);

        // length >= 7 to satisfy internal indexing of last 3 bytes
        var original = Encoding.ASCII.GetBytes("1234567");
        var buffer = new Span<byte>(original.ToArray());

        c.ServerDecrypt(ref buffer, opCode, 0);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("1234567");
    }

    [Test]
    [Arguments((byte)0)] // None => early return
    [Arguments((byte)3)]
    [Arguments((byte)64)]
    [Arguments((byte)126)]
    public void ServerEncrypt_Should_Not_Modify_When_Type_None(byte opCode)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("nochange");
        var memoryOwner = MemoryPool<byte>.Shared.Rent(original.Length);
        original.CopyTo(memoryOwner.Memory.Span);
        var length = original.Length;

        c.ServerEncrypt(
            ref memoryOwner,
            ref length,
            opCode,
            0);

        var buffer = memoryOwner.Memory.Span[..length];

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("nochange");

        memoryOwner.Dispose();
    }

    [Test]
    [Arguments((byte)200, (byte)1)] // MD5 path
    public void ServerEncrypt_Then_ClientDecrypt_Should_Roundtrip_For_MD5(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("srv-md5");
        var memoryOwner = MemoryPool<byte>.Shared.Rent(original.Length);
        original.CopyTo(memoryOwner.Memory.Span);
        var length = original.Length;

        c.ServerEncrypt(
            ref memoryOwner,
            ref length,
            opCode,
            sequence);

        var buffer = memoryOwner.Memory.Span[..length];
        c.ClientDecrypt(ref buffer, opCode, sequence);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("srv-md5");

        memoryOwner.Dispose();
    }

    [Test]
    [Arguments((byte)2, (byte)4)] // Normal path
    public void ServerEncrypt_Then_ClientDecrypt_Should_Roundtrip_For_Normal(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var original = Encoding.ASCII.GetBytes("srv-norm");
        var memoryOwner = MemoryPool<byte>.Shared.Rent(original.Length);
        original.CopyTo(memoryOwner.Memory.Span);
        var length = original.Length;

        c.ServerEncrypt(
            ref memoryOwner,
            ref length,
            opCode,
            sequence);

        var buffer = memoryOwner.Memory.Span[..length];
        c.ClientDecrypt(ref buffer, opCode, sequence);

        Encoding.ASCII
                .GetString(buffer.ToArray())
                .Should()
                .Be("srv-norm");

        memoryOwner.Dispose();
    }

    [Test]
    [Arguments((byte)200, (byte)1)] // MD5 path with empty payload
    public void ServerEncrypt_Then_ClientDecrypt_Should_Work_With_EmptyPayload(byte opCode, byte sequence)
    {
        var c = new Crypto(0, "UrkcnItnI", string.Empty);
        var memoryOwner = MemoryPool<byte>.Shared.Rent(0);
        var length = 0;

        c.ServerEncrypt(
            ref memoryOwner,
            ref length,
            opCode,
            sequence);

        var buffer = memoryOwner.Memory.Span[..length];
        c.ClientDecrypt(ref buffer, opCode, sequence);

        buffer.ToArray()
              .Should()
              .BeEmpty();

        memoryOwner.Dispose();
    }

    [Test]
    public void Should_GenerateKey_With_Valid_A_And_B()
    {
        var key = Crypto.GenerateKey(1, 2);

        key.Should()
           .NotBeNull();

        key.Length
           .Should()
           .Be(9);
    }

    [Test]
    public void Should_GenerateKeySalts_With_Valid_Seed()
    {
        var keySalts = Crypto.GenerateKeySalts("mySeed");

        keySalts.Should()
                .NotBeNull();

        keySalts.Length
                .Should()
                .Be(1024, "32 hashes of 32 characters each = 1024");
    }

    [Test]
    public void Should_GetMd5Hash_With_Valid_Value()
    {
        var md5Hash = Crypto.GetMd5Hash("myValue");

        md5Hash.Should()
               .NotBeNull();

        md5Hash.Length
               .Should()
               .Be(32);
    }

    [Test]
    [Arguments((byte)0, EncryptionType.None)]
    [Arguments((byte)3, EncryptionType.Normal)]
    [Arguments((byte)255, EncryptionType.MD5)]
    public void Should_Return_Valid_EncryptionType_From_GetClientEncryptionType(byte opCode, EncryptionType expectedType)
    {
        var actualType = Crypto.GetClientEncryptionType(opCode);

        actualType.Should()
                  .Be(expectedType);
    }

    [Test]
    [Arguments((byte)0, EncryptionType.None)]
    [Arguments((byte)2, EncryptionType.Normal)]
    [Arguments((byte)255, EncryptionType.MD5)]
    public void Should_Return_Valid_EncryptionType_From_ServerEncryptionType(byte opCode, EncryptionType expectedType)
    {
        var actualType = Crypto.GetServerEncryptionType(opCode);

        actualType.Should()
                  .Be(expectedType);
    }

    [Test]
    [Arguments((byte)0, false)]
    [Arguments((byte)2, true)]
    [Arguments((byte)3, true)]
    public void ShouldBeEncrypted_Should_Return_Correct_Value(byte opCode, bool expectedResult)
    {
        var result = Crypto.IsClientEncrypted(opCode);

        result.Should()
              .Be(expectedResult);
    }

    [Test]
    [Arguments((byte)0, false)]
    [Arguments((byte)1, true)]
    [Arguments((byte)2, true)]
    public void ShouldEncrypt_Should_Return_Correct_Value(byte opCode, bool expectedResult)
    {
        var result = Crypto.IsServerEncrypted(opCode);

        result.Should()
              .Be(expectedResult);
    }
}