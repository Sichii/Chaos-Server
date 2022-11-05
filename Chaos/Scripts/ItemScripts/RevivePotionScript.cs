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
    public class RevivePotionScript : ConfigurableItemScriptBase
    {
        protected EquipmentType EquipmentType { get; init; }

        public RevivePotionScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {
            if (!source.IsAlive)
            {
                //Let's restore their maximums
                source.StatSheet.AddHp(source.StatSheet.MaximumHp);
                source.StatSheet.AddHp(source.StatSheet.MaximumMp);

                //Refresh the users health bar
                source.Client.SendAttributes(StatUpdateType.Vitality);

                //Let's tell the player they have been revived
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived.");
            }
        }
    }
}
