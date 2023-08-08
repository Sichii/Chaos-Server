using Chaos.Cryptography.Abstractions.Definitions;
using FluentAssertions;
using Xunit;

// ReSharper disable ArrangeAttributes

namespace Chaos.Cryptography.Tests;

public sealed class CryptoTests
{
    private readonly Crypto Crypto = new(0, new byte[] { 1, 2, 3, 4, 5 });

    [Fact]
    public void Should_GenerateKey_With_Valid_A_And_B()
    {
        var key = Crypto.GenerateKey(1, 2);

        key.Should().NotBeNull();
        key.Length.Should().Be(9);
    }

    [Fact]
    public void Should_GenerateKeySalts_With_Valid_Seed()
    {
        var keySalts = Crypto.GenerateKeySalts("mySeed");

        keySalts.Should().NotBeNull();
        keySalts.Length.Should().Be(1024, "32 hashes of 32 characters each = 1024");
    }

    [Fact]
    public void Should_GetMd5Hash_With_Valid_Value()
    {
        var md5Hash = Crypto.GetMd5Hash("myValue");

        md5Hash.Should().NotBeNull();
        md5Hash.Length.Should().Be(32);
    }

    [Theory]
    [InlineData(0, EncryptionType.None)]
    [InlineData(3, EncryptionType.Normal)]
    [InlineData(255, EncryptionType.MD5)]
    public void Should_Return_Valid_EncryptionType_From_GetClientEncryptionType(byte opCode, EncryptionType expectedType)
    {
        var actualType = Crypto.GetClientEncryptionType(opCode);

        actualType.Should().Be(expectedType);
    }

    [Theory]
    [InlineData(0, EncryptionType.None)]
    [InlineData(2, EncryptionType.Normal)]
    [InlineData(255, EncryptionType.MD5)]
    public void Should_Return_Valid_EncryptionType_From_ServerEncryptionType(byte opCode, EncryptionType expectedType)
    {
        var actualType = Crypto.ServerEncryptionType(opCode);

        actualType.Should().Be(expectedType);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    public void ShouldBeEncrypted_Should_Return_Correct_Value(byte opCode, bool expectedResult)
    {
        var result = Crypto.ShouldBeEncrypted(opCode);
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    public void ShouldEncrypt_Should_Return_Correct_Value(byte opCode, bool expectedResult)
    {
        var result = Crypto.ShouldEncrypt(opCode);
        result.Should().Be(expectedResult);
    }
}