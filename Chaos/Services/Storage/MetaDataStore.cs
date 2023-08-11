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
    private readonly ConcurrentDictionary<string, IMetaDataDescriptor> MetaData;
    private readonly MetaDataStoreOptions Options;

    public MetaDataStore(
        ISimpleCacheProvider cacheProvider,
        IEntityRepository entityRepository,
        IOptions<MetaDataStoreOptions> options,
        ILogger<MetaDataStore> logger
    )
    {
        MetaData = new ConcurrentDictionary<string, IMetaDataDescriptor>(StringComparer.OrdinalIgnoreCase);
        Options = options.Value;
        Logger = logger;
        CacheProvider = cacheProvider;
        EntityRepository = entityRepository;

        Load();
    }

    /// <inheritdoc />
    public IMetaDataDescriptor Get(string name) =>
        MetaData.TryGetValue(name, out var metaData)
            ? metaData
            : throw new KeyNotFoundException($"MetaData with name \"{name}\" not found in cache");

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<IMetaDataDescriptor> GetEnumerator() => MetaData.Values.GetEnumerator();

    /// <inheritdoc />
    public void Load()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating metadata in parallel...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        Parallel.Invoke(
            LoadNationDescriptionMetaData,
            LoadItemMetaData,
            LoadAbilityMetaData,
            LoadEventMetaData,
            LoadMundaneIllustrationMeta);

        metricsLogger.LogInformation("Metadata generated");
    }

    protected virtual void LoadAbilityMetaData()
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

            switch (reqs.PrerequisiteSkillTemplateKeys.Count)
            {
                case 2:
                {
                    var obj = skillTemplateCache.Get(reqs.PrerequisiteSkillTemplateKeys.ElementAt(1));
                    req3 = (obj.Name, (byte)obj.Level);
                    goto case 1;
                }
                case 1:
                {
                    var obj = skillTemplateCache.Get(reqs.PrerequisiteSkillTemplateKeys.ElementAt(0));
                    req1 = (obj.Name, (byte)obj.Level);

                    break;
                }
            }

            switch (reqs.PrerequisiteSpellTemplateKeys.Count)
            {
                case 2:
                {
                    var obj = spellTemplateCache.Get(reqs.PrerequisiteSpellTemplateKeys.ElementAt(1));
                    req4 = (obj.Name, (byte)obj.Level);
                    goto case 1;
                }
                case 1:
                {
                    var obj = spellTemplateCache.Get(reqs.PrerequisiteSpellTemplateKeys.ElementAt(0));
                    req2 = (obj.Name, (byte)obj.Level);

                    break;
                }
            }

            var objs = new[] { req1, req2, req3, req4 }.Where(obj => obj is not null).ToList();

            var node = new AbilityMetaNode(template.Name, template is SkillTemplate, template.Class ?? BaseClass.Peasant)
            {
                Level = template.Level,
                RequiresMaster = false, //TODO: may need to implement this in ItemTemplate.LearningRequirements
                Ability = 0,
                IconId = template.PanelSprite,
                Str = (byte)(reqs.RequiredStats?.Str ?? 0),
                Int = (byte)(reqs.RequiredStats?.Int ?? 0),
                Wis = (byte)(reqs.RequiredStats?.Wis ?? 0),
                Con = (byte)(reqs.RequiredStats?.Con ?? 0),
                Dex = (byte)(reqs.RequiredStats?.Dex ?? 0),
                PreReq1Name = objs.ElementAtOrDefault(0)?.Name,
                //PreReq1Level = 0,
                PreReq2Name = objs.ElementAtOrDefault(1)?.Name,
                //PreReq2Level = 0,
                Description = template.Description
            };

            masterAbilityMetaData.AddNode(node);
        }

        foreach (var abilityMetaData in masterAbilityMetaData.Split())
            MetaData[abilityMetaData.Name] = abilityMetaData;

        metricsLogger.LogDebug("Ability metadata generated");
    }

    protected virtual void LoadEventMetaData()
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
            MetaData[eventMetaData.Name] = eventMetaData;

        metricsLogger.LogDebug("Event metadata generated");
    }

    protected virtual void LoadItemMetaData()
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

            if (!template.IsModifiable)
                continue;

            var prefixMutations = Options.PrefixMutators.SelectMany(mutator => mutator.Mutate(node, template));

            var suffixMutations = Options.SuffixMutators.SelectMany(mutator => mutator.Mutate(node, template));

            var prefixAndSuffixMutations = Options.PrefixMutators.SelectMany(mutator => mutator.Mutate(node, template))
                                                  .SelectMany(
                                                      mutated => Options.SuffixMutators.SelectMany(
                                                          mutator => mutator.Mutate(mutated, template)));

            var allMutations = prefixMutations.Concat(suffixMutations)
                                              .Concat(prefixAndSuffixMutations)
                                              .DistinctBy(n => n.Name);

            foreach (var mutation in allMutations)
                itemMetaNodes.AddNode(mutation);
        }

        foreach (var itemMetaData in itemMetaNodes.Split())
            MetaData[itemMetaData.Name] = itemMetaData;

        metricsLogger.LogDebug("Item metadata generated");
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

    protected virtual void LoadMundaneIllustrationMeta()
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
        MetaData[metaData.Name] = metaData;

        metricsLogger.LogDebug("Mundane illustration metadata generated");
    }

    protected virtual void LoadNationDescriptionMetaData()
    {
        Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
              .LogDebug("Generating nation description metadata...");

        var metricsLogger = Logger.WithTopics(Topics.Entities.MetaData, Topics.Actions.Create, Topics.Actions.Processing)
                                  .WithMetrics();

        var nations = Enum.GetValues<Nation>().OfType<Nation>();
        var nationDescriptionMetaData = new NationDescriptionMetaData();

        foreach (var nation in nations)
        {
            var node = new NationDescriptionMetaNode(nation);
            nationDescriptionMetaData.AddNode(node);
        }

        nationDescriptionMetaData.Compress();
        MetaData[nationDescriptionMetaData.Name] = nationDescriptionMetaData;

        metricsLogger.LogDebug("Nation description metadata generated");
    }
}