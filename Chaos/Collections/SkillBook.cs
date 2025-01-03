#region
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a panel of skills
/// </summary>
public sealed class SkillBook : KnowledgeBookBase<Skill>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SkillBook" /> class.
    /// </summary>
    /// <param name="skills">
    ///     The skills to populate the panel with
    /// </param>
    public SkillBook(IEnumerable<Skill>? skills = null)
        : base(PanelType.SkillBook, skills) { }
}