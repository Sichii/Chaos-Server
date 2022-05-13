using Chaos.Containers.Abstractions;
using Chaos.Core.Definitions;
using Chaos.Objects.Panel;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class SkillBook : PanelBase<Skill>
{
    public SkillBook()
        : base(
            PanelType.SkillBook,
            89,
            new byte[] { 0, 36, 72 }) { }
}