using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class LocalFunctionBenchmarks
{
    private const int NUM1 = 5;
    private const int NUM2 = 10;

    [Benchmark]
    public int Add() => NUM1 + NUM2;

    [Benchmark]
    public int AddLocal()
    {
        int InternalAddLocal() => NUM1 + NUM2;

        return InternalAddLocal();
    }

    [Benchmark]
    public int AddLocalNoScope()
    {
        int InternalAddLocalNoSope(int num1, int num2) => num1 + num2;

        return InternalAddLocalNoSope(NUM1, NUM2);
    }

    [Benchmark(Baseline = true)]
    public int AddMethodCall() => InternalAdd(NUM1, NUM2);

    [Benchmark]
    public int AddStaticLocal()
    {
        static int InternalAddStaticLocal(int num1, int num2) => num1 + num2;

        return InternalAddStaticLocal(NUM1, NUM2);
    }

    private int InternalAdd(int num1, int num2) => num1 + num2;
}