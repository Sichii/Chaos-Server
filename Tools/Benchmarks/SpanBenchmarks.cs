using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class SpanBenchmarks
{
    public short Int16 = 1;
    public int Int32 = 2;
    public int Index { get; set; }
    public Memory<byte> Memory { get; }

    public SpanBenchmarks()
    {
        var buffer = new byte[16];
        Memory = new Memory<byte>(buffer);
    }

    [Benchmark, BenchmarkCategory("Int16", "Assignment")]
    public void Int16Assignment()
    {
        var span = Memory.Span;
        span[Index++] = (byte)(Int16 >> 8);
        span[Index++] = (byte)Int16;
    }

    [Benchmark, BenchmarkCategory("Int16", "Assignment", "Fill")]
    public void Int16AssignmentFill()
    {
        var span = Memory.Span;

        for (; Index < 16;)
        {
            span[Index++] = (byte)(Int16 >> 8);
            span[Index++] = (byte)Int16;
        }
    }

    [Benchmark, BenchmarkCategory("Int16", "Marshal")]
    public void Int16Marshal()
    {
        var span = Memory.Span;
        MemoryMarshal.Write(span, ref Int16);
    }

    [Benchmark, BenchmarkCategory("Int16", "Marshal", "Fill")]
    public void Int16MarshalFill()
    {
        for (; Index < 16;)
        {
            var span = Memory[Index..].Span;
            MemoryMarshal.Write(span, ref Int16);
            Index += 2;
        }
    }

    [Benchmark, BenchmarkCategory("Int32", "Assignment")]
    public void Int32Assignment()
    {
        var span = Memory.Span;
        span[Index++] = (byte)(Int32 >> 24);
        span[Index++] = (byte)(Int32 >> 16);
        span[Index++] = (byte)(Int32 >> 8);
        span[Index++] = (byte)Int32;
    }

    [Benchmark, BenchmarkCategory("Int32", "Assignment", "Fill")]
    public void Int32AssignmentFill()
    {
        var span = Memory.Span;

        for (; Index < 16;)
        {
            span[Index++] = (byte)(Int32 >> 24);
            span[Index++] = (byte)(Int32 >> 16);
            span[Index++] = (byte)(Int32 >> 8);
            span[Index++] = (byte)Int32;
        }
    }

    [Benchmark, BenchmarkCategory("Int32", "Marshal")]
    public void Int32Marshal()
    {
        var span = Memory.Span;
        MemoryMarshal.Write(span, ref Int32);
    }

    [Benchmark, BenchmarkCategory("Int32", "Marshal", "Fill")]
    public void Int32MarshalFill()
    {
        for (; Index < 16;)
        {
            var span = Memory[Index..].Span;
            MemoryMarshal.Write(span, ref Int32);
            Index += 4;
        }
    }
}