using System.Text;
using Chaos.Core.Compression;
using Chaos.Cryptography.Definitions;

namespace Chaos.Objects;

public record Notice
{
    public uint CheckSum { get; }
    public byte[] Data { get; }

    public Notice(string noticeMessage, Encoding encoding)
    {
        var buffer = encoding.GetBytes(noticeMessage);
        CheckSum = buffer.Generate32();

        ZLIB.CompressInPlace(ref buffer);
        Data = buffer;
    }
}