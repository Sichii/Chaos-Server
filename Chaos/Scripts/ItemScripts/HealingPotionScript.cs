using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
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
    public class HealingPotionScript : ConfigurableItemScriptBase
    {
        protected int? HealAmount { get; init; }
        protected int? HealPercent { get; init; }

        public HealingPotionScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {
            if (source.IsAlive)
            {
                if (source.StatSheet.CurrentHp < source.StatSheet.MaximumHp)
                {
                    var amount = HealAmount ?? (source.StatSheet.MaximumHp / 100) * HealPercent;

                    //Let's add HP
                    source.StatSheet.AddHp(amount!.Value);

                    //Refresh the users health bar
                    source.Client.SendAttributes(StatUpdateType.Vitality);

                    //Let's tell the player they have been healed
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've been healed by " + amount + ".");

                    //Update inventory quantity
                    source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                }
            }
        }
    }
}
