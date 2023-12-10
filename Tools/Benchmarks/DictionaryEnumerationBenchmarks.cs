using System.Collections.Concurrent;
using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace Benchmarks;

[MemoryDiagnoser]

// ReSharper disable once ClassCanBeSealed.Global
public class DictionaryEnumerationBenchmarks
{
    private readonly ConcurrentDictionary<string, string> ConcurrentDictionary = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> NormalDictionary = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> ValuesList = new();
    private FrozenDictionary<string, string> FrozenDictionary;

    [Params(1000)]

    // ReSharper disable once UnassignedField.Global
    public int NumRecords;

    [Benchmark]
    public List<string> ConcurrentSelectBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            ConcurrentDictionary.Select(kvp => kvp.Value)
                                .ToList();

    [Benchmark]
    public List<string> ConcurrentValuesBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            ConcurrentDictionary.Values.ToList();

    [Benchmark]
    public List<string> FrozenSelectBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            FrozenDictionary.Select(kvp => kvp.Value)
                            .ToList();

    [Benchmark]
    public List<string> FrozenValuesBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            FrozenDictionary.Values.ToList();

    [Benchmark(Baseline = true)]
    public List<string> ListBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            ValuesList.ToList();

    [Benchmark]
    public List<string> NormalSelectBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            NormalDictionary.Select(kvp => kvp.Value)
                            .ToList();

    [Benchmark]
    public List<string> NormalValuesBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            NormalDictionary.Values.ToList();

    [GlobalSetup]
    public void Setup()
    {
        Randomizer.Seed = new Random(12345);

        var faker = new Faker();

        var randomKeys = Enumerable.Range(0, NumRecords)
                                   .Select(_ => faker.Random.String2(3, 15) + faker.UniqueIndex)
                                   .Append("test")
                                   .ToList();

        var randomValues = Enumerable.Range(0, NumRecords)
                                     .Select(_ => faker.Random.String2(10, 20))
                                     .Append("test")
                                     .ToList();

        foreach ((var key, var value) in randomKeys.Zip(randomValues))
        {
            NormalDictionary.Add(key, value);
            ConcurrentDictionary.TryAdd(key, value);
            ValuesList.Add(value);
        }

        FrozenDictionary = NormalDictionary.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}