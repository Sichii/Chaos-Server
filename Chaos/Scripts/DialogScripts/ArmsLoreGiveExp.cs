using Chaos.Factories.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class ArmsLoreGiveExpScript : DialogScriptBase
    {
        public ArmsLoreGiveExpScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.Legend.TryGetValue("arms", out var legendMark) && (legendMark.Count >= 6))
                return;

            source.Legend.AddOrAccumulate(new LegendMark("Learned Arms from Torrance", "arms", Common.Definitions.MarkIcon.Heart, Common.Definitions.MarkColor.White, 1, Time.GameTime.Now));
            source.GiveExp(250);
        }
    }
}
