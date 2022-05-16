using Chaos.Containers.Abstractions;
using Chaos.Objects.Panel;

namespace Chaos.Containers;

public class SpellBook : PanelBase<Spell>
{
    public SpellBook()
        : base(
            PanelType.SpellBook,
            90,
            new byte[] { 0, 36, 72 }) { }
}