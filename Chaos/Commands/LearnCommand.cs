using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Commands;

[Command("learn")]
public class LearnCommand : ICommand<Aisling>
{
    private readonly ISpellFactory SpellFactory;
    private readonly ISkillFactory SkillFactory;
    public LearnCommand(ISpellFactory spellFactory, ISkillFactory skillFactory)
    {
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
    }

    /// <inheritdoc />
    public void Execute(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGet<string>(0, out var type))
            return;

        if (!args.TryGet<string>(1, out var name))
            return;
        
        switch (type.ToLower())
        {
            case "spell":
                var spell = SpellFactory.Create(name);

                source.SpellBook.TryAddToNextSlot(spell);
                
                break;
            case "skill":
                var skill = SkillFactory.Create(name);

                source.SkillBook.TryAddToNextSlot(skill);
                
                break;
        }
    }
}