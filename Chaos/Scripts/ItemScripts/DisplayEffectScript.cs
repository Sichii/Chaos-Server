using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.ItemScripts
{
    public class DisplayEffectScript : ConfigurableItemScriptBase
    {

        protected Animation? Animation { get; init; }
        protected BodyAnimation? BodyAnimation { get; init; }
        protected byte? Sound { get; init; }

        public DisplayEffectScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {
            if (source.IsAlive)
            {
                if (Animation != null)
                    source.MapInstance.ShowAnimation(Animation.GetTargetedAnimation(source.Id));

                if (Sound.HasValue)
                    source.MapInstance.PlaySound(Sound.Value, source);

                if (BodyAnimation.HasValue)
                    source.AnimateBody(BodyAnimation.Value);

                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            }
        }
    }
}
