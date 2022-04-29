using Chaos.Containers.Abstractions;
using Chaos.Core.Definitions;
using Chaos.PanelObjects;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class SkillBook : PanelBase<Skill>
{
    public SkillBook(ILogger logger)
        : base(PanelType.SkillBook, 89, new byte[] { 0, 36, 72 }, logger) { }
}