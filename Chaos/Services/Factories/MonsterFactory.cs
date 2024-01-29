using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class MonsterFactory(
    ISimpleCache simpleCache,
    IScriptProvider scriptProvider,
    ILoggerFactory loggerFactory,
    ISkillFactory skillFactory,
    ISpellFactory spellFactory) : IMonsterFactory
{
    private readonly ILoggerFactory LoggerFactory = loggerFactory;
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;
    private readonly ISkillFactory SkillFactory = skillFactory;
    private readonly ISpellFactory SpellFactory = spellFactory;

    /// <inheritdoc />
    public Monster Create(
        string templateKey,
        MapInstance mapInstance,
        IPoint point,
        ICollection<string>? extraScriptKeys = null)
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

        return monster;
    }
}