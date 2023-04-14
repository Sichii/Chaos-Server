/*using Chaos;
using Chaos.Data;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using Xunit;

namespace Experiments;

public sealed class AIMapGenerator
{
    [Fact]
    public Task AiStuff()
    {
        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            #if DEBUG
                            .AddJsonFile("appsettings.local.json")
                            #else
                            //.AddJsonFile("appsettings.prod.json")
                            .AddJsonFile("appsettings.local.json")
                            #endif
                            .Build();

        var services = new ServiceCollection();
        var startup = new Startup(configuration);
        startup.ConfigureServices(services);

        var serverCtx = new CancellationTokenSource();

        services.AddSingleton(serverCtx);

        var provider = services.BuildServiceProvider();

        var simpleCacheProvider = provider.GetRequiredService<ISimpleCacheProvider>();
        var mapCache = simpleCacheProvider.GetCache<MapTemplate>();
        mapCache.ForceLoad();

        var modelInputs = mapCache.Select(
            mapTemplate => new ModelInput
            {
                Tiles = mapTemplate.Tiles,
                Labels = new[] { "" }
            });

        var context = new MLContext();
        var data = context.Data.LoadFromEnumerable(modelInputs);

        return Task.CompletedTask;

        /*
        // Define the training pipeline
        var pipeline = context.Transforms.Conversion.MapValueToKey("Label")
                                .Append(context.Transforms.Categorical.OneHotEncoding(new[] { new InputOutputColumnPair(nameof(Tile.Id)) }))
                                .Append(context.Transforms.Concatenate("Features", nameof(Tile.Id) + "Encoded"))
                                .Append(context.Transforms.NormalizeMinMax("Features"))
                                .Append(context.Transforms.Conversion.MapKeyToValue("Label"))
                                .Append(context.Transforms.CopyColumns("Label", "Label", new[] { nameof(Tile.Id) }, keyToValue: true))
                                .Append(context.Model.FeedForward("Features", "Label", numberOfLayers: 2, activation: new TanhActivationFunction(), learningRate: 0.01f, batchSize: 10, numIterations: 10))
                                .Append(context.Transforms.Conversion.MapKeyToValue("Label"));

        // Train the model
        var model = pipeline.Fit(data);

        // Save the model to a file
        using (var fileStream = new FileStream("tile_cnn.zip", FileMode.Create, FileAccess.Write, FileShare.Write))
            mlContext.Model.Save(model, data.Schema, fileStream);
        #1#
    }

    public sealed class ModelInput
    {
        public required string[] Labels { get; set; }
        public required Tile[,] Tiles { get; set; }
    }
}*/

