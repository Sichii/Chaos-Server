using Chaos.Common.Definitions;
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
    public class ToxicPotionScript : ConfigurableItemScriptBase
    {
        protected int? DmgAmount { get; init; }
        protected int? DmgPercent { get; init; }

        public ToxicPotionScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {
            if (source.IsAlive)
            {
                var amount = DmgAmount ?? (source.StatSheet.MaximumHp / 100) * DmgPercent;

                //Let's remove Hp
                source.StatSheet.SubtractHp(amount!.Value);

                //Refresh the users health bar
                source.Client.SendAttributes(StatUpdateType.Vitality);

                //Let's tell the player they have been healed
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A foul taste stings your tounge. Lose " + amount + " health.");

                //Update inventory quantity
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            }
        }
    }
}
