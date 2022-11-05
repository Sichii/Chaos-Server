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
    public class MessagePopupScript : ConfigurableItemScriptBase
    {
        protected string message { get; init; }
        protected bool showToNearby { get; init; }

        public MessagePopupScript(Item subject) : base(subject)
        {
        }

        public override void OnUse(Aisling source)
        {
            if (!showToNearby)
                source.Client.SendServerMessage(ServerMessageType.WoodenBoard, message);
            else
                foreach (var aisling in source.MapInstance.GetEntitiesWithinRange<Aisling>(source))
                    aisling.Client.SendServerMessage(ServerMessageType.WoodenBoard, message);
        }
    }
}
