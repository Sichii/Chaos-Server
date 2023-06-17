using System.Text;
using Chaos.Cryptography;
using Chaos.IO.Compression;
using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Entities;

/// <inheritdoc />
public sealed record Notice : INotice
{
    /// <inheritdoc />
    public uint CheckSum { get; }
    /// <inheritdoc />
    public byte[] Data { get; }

    /// <summary>
    ///     Creates a new <see cref="Notice" /> instance
    /// </summary>
    /// <param name="noticeMessage">The message displayed to the client at the login screen</param>
    public Notice(string noticeMessage)
    {
        var encoding = Encoding.GetEncoding(949);
        var buffer = encoding.GetBytes(noticeMessage);
        CheckSum = Crc.Generate32(buffer);

        ZLIB.Compress(ref buffer);
        Data = buffer;
    }
}