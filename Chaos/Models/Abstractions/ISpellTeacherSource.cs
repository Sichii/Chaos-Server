using Chaos.Models.Panel;

namespace Chaos.Models.Abstractions;

public interface ISpellTeacherSource : IDialogSourceEntity
{
    ICollection<Spell> SpellsToTeach { get; }
    bool TryGetSpell(string spellName, [MaybeNullWhen(false)] out Spell spell);
}