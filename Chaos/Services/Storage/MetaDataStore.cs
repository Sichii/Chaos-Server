using System.Collections.Frozen;
using Chaos.Common.Definitions;
using Chaos.MetaData.Abstractions;
using Chaos.MetaData.ClassMetaData;
using Chaos.MetaData.EventMetadata;
using Chaos.MetaData.ItemMetadata;
using Chaos.MetaData.MundaneMetadata;
using Chaos.MetaData.NationMetaData;
using Chaos.Models.Templates;
using Chaos.Models.Templates.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Schemas.MetaData;
using Chaos.Services.Storage.Abstractions;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class MetaDataStore : IMetaDataStore
{
    private readonly ISimpleCacheProvider CacheProvider;
    private readonly IEntityRepository EntityRepository;

    private readonly ILogger<MetaDataStore> Logger;

    private readonly FrozenDictionary<string, IMetaDataDescriptor> MetaData;
    private readonly MetaDataStoreOptions Options;

    public MetaDataStore(
        ISimpleCacheProvider cacheProvider,
        IEntityRepository entityRepository,
        IOptions<MetaDataStoreOptions> options,
        ILogger<MetaDataStore> logger)
    {
        Options = options.Value;
        Logger = logger;
        CacheProvider = cacheProvider;
        EntityRepository = entityRepository;

        MetaData = LoadMetaData()
            .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public IMetaDataDescriptor Get(string name)
        => MetaData.TryGetValue(name, out var metaData)
            ? metaData
            : throw new KeyNotFoundException($"MetaData with name \"{name}\" not found in cache");

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<IMetaDataDescriptor> GetEnumerator() => ((IEnumerable<IMetaDataDescriptor>)MetaData.Values).GetEnumerator();

    protected virtual IEnumerable<IMetaDataDescriptor> LoadAbilityMetaData()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating ability metadata...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        var skillTemplateCache = CacheProvider.GetCache<SkillTemplate>();
        var spellTemplateCache = CacheProvider.GetCache<SpellTemplate>();

        Parallel.Invoke(skillTemplateCache.ForceLoad, spellTemplateCache.ForceLoad);

        var masterAbilityMetaData = new AbilityMetaNodeCollection();

        foreach (var template in skillTemplateCache.Concat<PanelEntityTemplateBase>(spellTemplateCache))
        {
            var reqs = (template as SkillTemplate)?.LearningRequirements ?? (template as SpellTemplate)?.LearningRequirements;

            if (reqs == null)
                continue;

            (string Name, byte Level)? req1 = null;
            (string Name, byte Level)? req2 = null;
            (string Name, byte Level)? req3 = null;
            (string Name, byte Level)? req4 = null;

            switch (reqs.PrerequisiteSkills.Count)
            {
                case 2:
                {
                    var requirement = reqs.PrerequisiteSkills.ElementAt(1);
                    var obj = skillTemplateCache.Get(requirement.TemplateKey);
                    req3 = (obj.Name, requirement.Level ?? obj.MaxLevel);
                    goto case 1;
                }
                case 1:
                {
                    var requirement = reqs.PrerequisiteSkills.ElementAt(0);
                    var obj = skillTemplateCache.Get(requirement.TemplateKey);
                    req1 = (obj.Name, requirement.Level ?? obj.MaxLevel);

                    break;
                }
            }

            switch (reqs.PrerequisiteSpells.Count)
            {
                case 2:
                {
                    var requirement = reqs.PrerequisiteSpells.ElementAt(1);
                    var obj = spellTemplateCache.Get(requirement.TemplateKey);
                    req4 = (obj.Name, requirement.Level ?? obj.MaxLevel);
                    goto case 1;
                }
                case 1:
                {
                    var requirement = reqs.PrerequisiteSpells.ElementAt(0);
                    var obj = spellTemplateCache.Get(requirement.TemplateKey);
                    req2 = (obj.Name, requirement.Level ?? obj.MaxLevel);

                    break;
                }
            }

            var objs = new[]
                {
                    req1,
                    req2,
                    req3,
                    req4
                }.Where(obj => obj is not null)
                 .ToList();

            var node = new AbilityMetaNode(template.Name, template is SkillTemplate, template.Class ?? BaseClass.Peasant)
            {
                Level = template.Level,
                RequiresMaster = template.RequiresMaster,
                Ability = 0,
                IconId = template.PanelSprite,
                Str = (byte)(reqs.RequiredStats?.Str ?? 0),
                Int = (byte)(reqs.RequiredStats?.Int ?? 0),
                Wis = (byte)(reqs.RequiredStats?.Wis ?? 0),
                Con = (byte)(reqs.RequiredStats?.Con ?? 0),
                Dex = (byte)(reqs.RequiredStats?.Dex ?? 0),
                PreReq1Name = objs.ElementAtOrDefault(0)
                                  ?.Name,

                PreReq1Level = objs.ElementAtOrDefault(0)
                                   ?.Level,
                PreReq2Name = objs.ElementAtOrDefault(1)
                                  ?.Name,

                PreReq2Level = objs.ElementAtOrDefault(1)
                                   ?.Level,
                Description = template.Description
            };

            masterAbilityMetaData.AddNode(node);
        }

        foreach (var abilityMetaData in masterAbilityMetaData.Split())
            yield return abilityMetaData;

        metricsLogger.LogDebug("Ability metadata generated");
    }

    protected virtual IEnumerable<IMetaDataDescriptor> LoadEventMetaData()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating event metadata...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        var eventMetas = LoadMetaFromPath<EventMetaSchema>(Options.EventMetaPath);
        var eventMetaNodes = new EventMetaNodeCollection();

        foreach (var eventMeta in eventMetas)
        {
            string? classStr = null;
            string? circlesStr = null;
            var page = eventMeta.PageOverride;

            if (eventMeta.QualifyingClasses is { Count: > 0 })
                classStr = string.Join("", eventMeta.QualifyingClasses.Select(c => (int)c));

            if (eventMeta.QualifyingCircles is { Count: > 0 })
            {
                circlesStr = string.Join("", eventMeta.QualifyingCircles!.Select(c => (int)c));
                page ??= (int)eventMeta.QualifyingCircles!.Min();
            }

            var node = new EventMetaNode(eventMeta.Title, page ?? 1)
            {
                Id = eventMeta.Id,
                QualifyingClasses = classStr,
                QualifyingCircles = circlesStr,
                Rewards = eventMeta.Rewards,
                PrerequisiteEventId = eventMeta.PrerequisiteEventId,
                Summary = eventMeta.Summary,
                Result = eventMeta.Result
            };

            eventMetaNodes.AddNode(node);
        }

        foreach (var eventMetaData in eventMetaNodes.Split())
            yield return eventMetaData;

        metricsLogger.LogDebug("Event metadata generated");
    }

    protected virtual IEnumerable<IMetaDataDescriptor> LoadItemMetaData()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating item metadata...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        var itemTemplateCache = CacheProvider.GetCache<ItemTemplate>();
        itemTemplateCache.ForceLoad();

        var itemMetaNodes = new ItemMetaNodeCollection();

        foreach (var template in itemTemplateCache)
        {
            var node = new ItemMetaNode(template.Name)
            {
                Level = template.Level,
                Class = template.Class ?? BaseClass.Peasant,
                Weight = template.Weight,
                Description = template.Description ?? string.Empty,
                Category = template.Category
            };

            //add basic node
            itemMetaNodes.AddNode(node);

            if (template.IsDyeable)
                foreach (var color in Enum.GetNames<DisplayColor>())
                    itemMetaNodes.AddNode(
                        node with
                        {
                            Name = $"{color} {node.Name}"
                        });

            if (!template.IsModifiable)
                continue;

            var prefixMutations = Options.PrefixMutators.SelectMany(mutator => mutator.Mutate(node, template));

            var suffixMutations = Options.SuffixMutators.SelectMany(mutator => mutator.Mutate(node, template));

            var prefixAndSuffixMutations = Options.PrefixMutators
                                                  .SelectMany(mutator => mutator.Mutate(node, template))
                                                  .SelectMany(
                                                      mutated => Options.SuffixMutators.SelectMany(
                                                          mutator => mutator.Mutate(mutated, template)))
                                                  .ToList();

            var allMutations = prefixMutations.Concat(suffixMutations)
                                              .Concat(prefixAndSuffixMutations);

            if (template.IsDyeable)
                allMutations = allMutations.SelectMany(
                    mutated => Enum.GetNames<DisplayColor>()
                                   .Select(
                                       colorName => mutated with
                                       {
                                           Name = $"{colorName} {mutated.Name}"
                                       }));

            foreach (var mutation in allMutations.DistinctBy(n => n.Name))
                itemMetaNodes.AddNode(mutation);
        }

        foreach (var itemMetaData in itemMetaNodes.Split())
            yield return itemMetaData;

        metricsLogger.LogDebug("Item metadata generated");
    }

    public IDictionary<string, IMetaDataDescriptor> LoadMetaData()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating metadata in parallel...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        var ret = new ConcurrentDictionary<string, IMetaDataDescriptor>();

        //load metadata in parallel
        Parallel.Invoke(
            () =>
            {
                foreach (var metaData in LoadAbilityMetaData())
                    ret.TryAdd(metaData.Name, metaData);
            },
            () =>
            {
                foreach (var metaData in LoadEventMetaData())
                    ret.TryAdd(metaData.Name, metaData);
            },
            () =>
            {
                foreach (var metaData in LoadItemMetaData())
                    ret.TryAdd(metaData.Name, metaData);
            },
            () =>
            {
                var metaData = LoadMundaneIllustrationMeta();
                ret.TryAdd(metaData.Name, metaData);
            },
            () =>
            {
                var metaData = LoadNationDescriptionMetaData();
                ret.TryAdd(metaData.Name, metaData);
            });

        metricsLogger.LogInformation("Metadata generated");

        return ret;
    }

    protected virtual IEnumerable<T> LoadMetaFromPath<T>(string path)
    {
        var typeName = typeof(T).Name;

        if (string.IsNullOrEmpty(path))
        {
            Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                  .LogWarning("Metadata path is empty, no {@TypeName} will be generated", typeName);

            return Enumerable.Empty<T>();
        }

        var metaDir = Path.GetDirectoryName(path);

        if (!Directory.Exists(metaDir))
            Directory.CreateDirectory(metaDir!);

        if (!File.Exists(path))
        {
            Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                  .LogWarning("File not found at path {@MetaPath}, no {@TypeName} will be generated", path, typeName);

            return Enumerable.Empty<T>();
        }

        return EntityRepository.LoadMany<T>(path);
    }

    protected virtual IMetaDataDescriptor LoadMundaneIllustrationMeta()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating mundane illustration metadata...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        var mundaneIllustrationMetas = LoadMetaFromPath<MundaneIllustrationMetaSchema>(Options.MundaneIllustrationMetaPath);
        var metaData = new MundaneIllustrationMetaData();

        foreach (var mundaneIllustrationMeta in mundaneIllustrationMetas)
        {
            var node = new MundaneIllustrationMetaNode(mundaneIllustrationMeta.Name, mundaneIllustrationMeta.ImageName);
            metaData.AddNode(node);
        }

        metaData.Compress();

        metricsLogger.LogDebug("Mundane illustration metadata generated");

        return metaData;
    }

    protected virtual IMetaDataDescriptor LoadNationDescriptionMetaData()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating nation description metadata...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        var nations = Enum.GetValues<Nation>()
                          .OfType<Nation>();
        var nationDescriptionMetaData = new NationDescriptionMetaData();

        foreach (var nation in nations)
        {
            var node = new NationDescriptionMetaNode(nation);
            nationDescriptionMetaData.AddNode(node);
        }

        nationDescriptionMetaData.Compress();

        metricsLogger.LogDebug("Nation description metadata generated");

        return nationDescriptionMetaData;
    }
}