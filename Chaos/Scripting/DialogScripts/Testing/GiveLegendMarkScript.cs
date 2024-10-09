using Chaos.DarkAges.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
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