using System.Collections.Concurrent;
using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace Benchmarks;

[MemoryDiagnoser]
public class DictionaryLookupBenchmarks
{
    private readonly ConcurrentDictionary<string, string> ConcurrentDictionary = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> NormalDictionary = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> ValuesList = new();
    private FrozenDictionary<string, string> FrozenDictionary;

    // ReSharper disable once UnassignedField.Global
    [Params(1000)]
    public int NumRecords;

    [Benchmark]
    public string ConcurrentLookupBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            ConcurrentDictionary["test"];

    [Benchmark]
    public string FrozenLookupBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            FrozenDictionary["test"];

    [Benchmark(Baseline = true)]
    public string NormalLookupBenchmark()
        =>

            // ReSharper disable once UnusedVariable
            NormalDictionary["test"];

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