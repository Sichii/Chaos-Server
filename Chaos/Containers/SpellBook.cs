using Chaos.Containers.Abstractions;
using Chaos.Core.Definitions;
using Chaos.PanelObjects;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class SpellBook : PanelBase<Spell>
{
    public SpellBook(ILogger logger)
        : base(PanelType.SpellBook, 90, new byte[] { 0, 36, 72 }, logger) { }
}