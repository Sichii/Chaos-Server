using System.Text;
using Chaos.Extensions.Cryptography;
using Chaos.IO.Compression;
using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Entities;

public sealed record Notice : INotice
{
    public uint CheckSum { get; }
    public byte[] Data { get; }

    public Notice(string noticeMessage)
    {
        var encoding = Encoding.GetEncoding(949);
        var buffer = encoding.GetBytes(noticeMessage);
        CheckSum = Crc.Generate32(buffer);

        ZLIB.Compress(ref buffer);
        Data = buffer;
    }
}