using Chaos.Common.Definitions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Testing;

public class GiveLegendMarkScript : DialogScriptBase
{
    /// <inheritdoc />
    public GiveLegendMarkScript(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDisplayed(Aisling source)
    {
        var legendMark = new LegendMark(
            "This is a test",
            "Test1",
            MarkIcon.Victory,
            MarkColor.LightPurple,
            1,
            GameTime.Now);

        source.Legend.AddOrAccumulate(legendMark);
    }
}