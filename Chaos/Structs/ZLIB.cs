using System.IO;
using zlib;

namespace Chaos
{
    internal static class ZLIB
    {
        internal static MemoryStream Compress(byte[] buffer)
        {
            MemoryStream compressed = new MemoryStream();

            using (MemoryStream uncompressed = new MemoryStream(buffer))
            using (ZOutputStream compressor = new ZOutputStream(compressed, -1))
            {
                uncompressed.Seek(0L, SeekOrigin.Begin);
                byte[] buffer1 = new byte[2000];
                int count;

                while ((count = uncompressed.Read(buffer1, 0, 2000)) > 0)
                    compressor.Write(buffer1, 0, count);

                compressor.Flush();
            }

            return compressed;
        }

        internal static MemoryStream Decompress(byte[] buffer)
        {
            MemoryStream uncompressed = new MemoryStream();
            using (ZOutputStream decompressor = new ZOutputStream(uncompressed))
            {
                int offset = 0;

                while (offset < buffer.Length)
                {
                    int num = buffer.Length - offset;
                    int count = num > 2000 ? 2000 : num;
                    decompressor.Write(buffer, offset, count);
                    offset += count;
                }

                decompressor.Flush();
                uncompressed.Seek(0L, SeekOrigin.Begin);
            }

            return uncompressed;
        }

        internal static void Compress(string inFile, string outFile)
        {
            FileStream uncompressed = new FileStream(outFile, FileMode.Create);

            using (ZOutputStream compressor = new ZOutputStream(uncompressed, -1))
            using (FileStream compressed = new FileStream(inFile, FileMode.Open))
                CopyWriteStream(uncompressed, compressor);
        }

        internal static void Decompress(string inFile, string outFile)
        {
            FileStream compressed = new FileStream(outFile, FileMode.Create);

            using (ZOutputStream decompressor = new ZOutputStream(compressed))
            using (FileStream uncompressed = new FileStream(inFile, FileMode.Open))
                CopyWriteStream(uncompressed, decompressor);
        }

        internal static void CopyWriteStream(Stream input, Stream output)
        {
            try
            {
                byte[] buffer = new byte[2000];
                int count;

                while ((count = input.Read(buffer, 0, 2000)) > 0)
                    output.Write(buffer, 0, count);

                output.Flush();
            }
            catch { }
        }
    }
}
