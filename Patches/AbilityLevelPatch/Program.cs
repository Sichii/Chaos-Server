// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Services.Storage.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

#pragma warning disable CS1591

var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
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

var jsonOptions = provider.GetRequiredService<IOptions<JsonSerializerOptions>>()
                          .Value;

var spellTemplateCacheOptions = provider.GetRequiredService<IOptions<SpellTemplateCacheOptions>>()
                                        .Value;

var skillTemplateCacheOptions = provider.GetRequiredService<IOptions<SkillTemplateCacheOptions>>()
                                        .Value;

var filePaths = Directory.EnumerateFiles(spellTemplateCacheOptions.Directory, "*.json", SearchOption.AllDirectories)
                         .Concat(Directory.EnumerateFiles(skillTemplateCacheOptions.Directory, "*.json", SearchOption.AllDirectories));

await Parallel.ForEachAsync(
    filePaths,
    async (path, _) =>
    {
        var json = await JsonSerializerEx.DeserializeAsync<JsonObject>(path, jsonOptions);

        if (json is null)
            return;

        var changed = false;

        foreach (var kvp in json)
        {
            if (!kvp.Key.EqualsI("learningRequirements") || kvp.Value is null)
                continue;

            var reqsObj = kvp.Value.AsObject();

            if (reqsObj.TryGetPropertyValue("prerequisiteSkillTemplateKeys", out var prereqSkillsNode) && prereqSkillsNode is not null)
            {
                reqsObj.Remove("prerequisiteSkillTemplateKeys");
                changed = true;

                var newArr = new JsonArray();

                foreach (var entry in prereqSkillsNode.AsArray())
                {
                    if (entry is null)
                        continue;

                    var newEntry = new JsonObject();
                    var templateKey = entry.GetValue<string>();

                    newEntry.Add("templateKey", templateKey);
                    newArr.Add(newEntry);
                }

                reqsObj.Add("prerequisiteSkills", newArr);
            }

            if (reqsObj.TryGetPropertyValue("prerequisiteSpellTemplateKeys", out var prereqSpellsNode) && prereqSpellsNode is not null)
            {
                reqsObj.Remove("prerequisiteSpellTemplateKeys");
                changed = true;

                var newArr = new JsonArray();

                foreach (var entry in prereqSpellsNode.AsArray())
                {
                    if (entry is null)
                        continue;

                    var newEntry = new JsonObject();
                    var templateKey = entry.GetValue<string>();

                    newEntry.Add("templateKey", templateKey);
                    newArr.Add(newEntry);
                }

                reqsObj.Add("prerequisiteSpells", newArr);
            }
        }

        if (changed)
            await JsonSerializerEx.SerializeAsync(path, json, jsonOptions);
    });

/*
var itemTemplateCacheOptions = provider.GetRequiredService<IOptions<ItemTemplateCacheOptions>>()
                                       .Value;
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
}*/

#pragma warning disable CA1050
public enum OldColors : byte
    #pragma warning restore CA1050
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