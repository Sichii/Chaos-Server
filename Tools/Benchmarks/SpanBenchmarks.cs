using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

// ReSharper disable ClassCanBeSealed.Global

namespace Benchmarks;

[MemoryDiagnoser]
public class SpanBenchmarks
{
    public short Int16 = 1;
    public int Int32 = 2;
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
        var index = 0;

        span[index++] = (byte)(Int16 >> 8);
        span[index] = (byte)Int16;
    }

    [Benchmark, BenchmarkCategory("Int16", "Assignment", "Fill")]
    public void Int16AssignmentFill()
    {
        var span = Memory.Span;
        var index = 0;

        for (; index < 16;)
        {
            span[index++] = (byte)(Int16 >> 8);
            span[index++] = (byte)Int16;
        }
    }

    [Benchmark, BenchmarkCategory("Int16", "Marshal")]
    public void Int16Marshal()
    {
        var span = Memory.Span;
        MemoryMarshal.Write(span, in Int16);
    }

    [Benchmark, BenchmarkCategory("Int16", "Marshal", "Fill")]
    public void Int16MarshalFill()
    {
        var buffer = Memory.Span;

        for (var index = 0; index < 16; index += 2)
            MemoryMarshal.Write(buffer[index..], in Int16);
    }

    [Benchmark, BenchmarkCategory("Int32", "Assignment")]
    public void Int32Assignment()
    {
        var span = Memory.Span;
        var index = 0;

        span[index++] = (byte)(Int32 >> 24);
        span[index++] = (byte)(Int32 >> 16);
        span[index++] = (byte)(Int32 >> 8);
        span[index] = (byte)Int32;
    }

    [Benchmark, BenchmarkCategory("Int32", "Assignment", "Fill")]
    public void Int32AssignmentFill()
    {
        var span = Memory.Span;
        var index = 0;

        for (; index < 16;)
        {
            span[index++] = (byte)(Int32 >> 24);
            span[index++] = (byte)(Int32 >> 16);
            span[index++] = (byte)(Int32 >> 8);
            span[index++] = (byte)Int32;
        }
    }

    [Benchmark, BenchmarkCategory("Int32", "Marshal")]
    public void Int32Marshal()
    {
        var span = Memory.Span;
        MemoryMarshal.Write(span, in Int32);
    }

    [Benchmark, BenchmarkCategory("Int32", "Marshal", "Fill")]
    public void Int32MarshalFill()
    {
        var buffer = Memory.Span;

        for (var index = 0; index < 16; index += 4)
            MemoryMarshal.Write(buffer[index..], in Int32);
    }
}