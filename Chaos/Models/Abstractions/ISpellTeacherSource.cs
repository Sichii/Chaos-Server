using Chaos.Models.Panel;

namespace Chaos.Models.Abstractions;

public interface ISpellTeacherSource : IDialogSourceEntity
{
    ICollection<Spell> SpellsToTeach { get; }
}