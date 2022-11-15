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
    public class ManaPotionScript : ConfigurableItemScriptBase
    {
        protected int? ManaAmount { get; init; }
        protected int? ManaPercent { get; init; }

        public ManaPotionScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {
            if (source.IsAlive)
            {
                if (source.StatSheet.CurrentMp < source.StatSheet.MaximumMp)
                {
                    var amount = ManaAmount ?? (source.StatSheet.MaximumMp / 100) * ManaPercent;

                    //Let's add HP
                    source.StatSheet.AddMp(amount!.Value);

                    //Refresh the users health bar
                    source.Client.SendAttributes(StatUpdateType.Vitality);

                    //Let's tell the player they have been healed
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your mana reserves grow a wee bit stronger.");

                    //Update inventory quantity
                    source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                }
            }
        }

    }
}
