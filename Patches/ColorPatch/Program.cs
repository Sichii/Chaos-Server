// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile("appsettings.local.json")
                    //other configurations
                    .Build();

var services = new ServiceCollection();

services.AddLogging();
services.AddChaosOptions(configuration);
services.AddJsonSerializerOptions();
services.AddStorage();

await using var provider = services.BuildServiceProvider();

var jsonOptions = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
var itemTemplateCacheOptions = provider.GetRequiredService<IOptions<ItemTemplateCacheOptions>>().Value;
var filePaths = Directory.EnumerateFiles(itemTemplateCacheOptions.Directory, "*.json", SearchOption.AllDirectories);

foreach (var path in filePaths)
{
    var itemTemplate = await JsonSerializerEx.DeserializeAsync<JsonObject>(path, jsonOptions);

    if (itemTemplate is null)
        continue;

    if (itemTemplate.TryGetPropertyValue("color", out var node))
    {
        var oldColorStr = node!.GetValue<string>();

        if (!Enum.TryParse<OldColors>(oldColorStr, out var oldColor))
            goto pantsColor;

        if (oldColor == OldColors.Default)
            goto pantsColor;

        var newColor = (DisplayColor)(byte)oldColor;

        itemTemplate["color"] = newColor.ToString();
        await JsonSerializerEx.SerializeAsync(path, itemTemplate, jsonOptions);
    }

    pantsColor:

    if (itemTemplate.TryGetPropertyValue("pantscolor", out node))
    {
        var oldColorStr = node!.GetValue<string>();

        if (!Enum.TryParse<OldColors>(oldColorStr, out var oldColor))
            continue;

        if (oldColor == OldColors.Default)
            continue;

        var newColor = (DisplayColor)(byte)oldColor;

        itemTemplate["pantsColor"] = newColor.ToString();
        await JsonSerializerEx.SerializeAsync(path, itemTemplate, jsonOptions);
    }
}

public enum OldColors : byte
{
    Default,
    Black,
    Red,
    Orange,
    Blonde,
    Cyan,
    Blue,
    Mulberry,
    Olive,
    Green,
    Fire,
    Brown,
    Grey,
    Navy,
    Tan,
    White,
    Pink,
    Chartreuse,
    Golden,
    Lemon,
    Royal,
    Platinum,
    Lilac,
    Fuchsia,
    Magenta,
    Peacock,
    NeonPink,
    Arctic,
    Mauve,
    NeonOrange,
    Sky,
    NeonGreen,
    Pistachio,
    Corn,
    Cerulean,
    Chocolate,
    Ruby,
    Hunter,
    Crimson,
    Ocean,
    Ginger,
    Mustard,
    Apple,
    Leaf,
    Cobalt,
    Strawberry,
    Unusual,
    Sea,
    Harlequin,
    Amethyst,
    NeonRed,
    NeonYellow,
    Rose,
    Salmon,
    Scarlet,
    Honey
}