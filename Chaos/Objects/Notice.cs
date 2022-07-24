using System.Text;
using Chaos.Cryptography.Extensions;
using Chaos.IO.Compression;

namespace Chaos.Objects;

public record Notice
{
    public uint CheckSum { get; }
    public byte[] Data { get; }

    public Notice(string noticeMessage)
    {
        var encoding = Encoding.GetEncoding(949);
        var buffer = encoding.GetBytes(noticeMessage);
        CheckSum = buffer.Generate32();

        ZLIB.Compress(ref buffer);
        Data = buffer;
    }
}