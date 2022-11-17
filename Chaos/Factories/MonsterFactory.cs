using Chaos.Containers;
using Chaos.Factories.Abstractions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public sealed class MonsterFactory : IMonsterFactory
{
    private readonly ILogger Logger;
    private readonly ILoggerFactory LoggerFactory;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

    public MonsterFactory(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<MonsterFactory> logger,
        ILoggerFactory loggerFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
        LoggerFactory = loggerFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
    }

    /// <inheritdoc />
    public Monster Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null
    )
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.Get<MonsterTemplate>(templateKey);
        var logger = LoggerFactory.CreateLogger<Monster>();

        var monster = new Monster(
            template,
            mapInstance,
            point,
            logger,
            ScriptProvider,
            extraScriptKeys);

        foreach (var skillTemplateKey in monster.Template.SkillTemplateKeys)
        {
            var skill = SkillFactory.CreateFaux(skillTemplateKey);
            monster.Skills.Add(skill);
        }

        foreach (var spellTemplateKey in monster.Template.SpellTemplateKeys)
        {
            var spell = SpellFactory.CreateFaux(spellTemplateKey);
            monster.Spells.Add(spell);
        }

        Logger.LogTrace(
            "Created monster {MonsterName} at location {Location}",
            monster.Name,
            Location.From(monster));

        return monster;
    }
}