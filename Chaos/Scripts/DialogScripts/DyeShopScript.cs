using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class DyeShopScript : DialogScriptBase
    {
        private readonly InputCollector inputCollector;
        private DisplayColor? displayColor;
        private Item? item;

        public DyeShopScript(Dialog subject) : base(subject)
        {
            inputCollector = new InputCollectorBuilder()
                .RequestTextInput(DialogString.From(RequestColorInput))
                .HandleInput(HandleColorInput)
                .RequestOptionSelection(DialogString.From(() => $"{item!.DisplayName} will be dyed {displayColor}. Is this correct?"), DialogString.Yes, DialogString.No)
                .HandleInput(HandleConfirmation).Build();
        }

        public string RequestColorInput()
        {
            var builder = new StringBuilder();
            builder.AppendLine("What color would you like this item to be?");
            builder.AppendLine("These are the available colors. ");
            var s = Enum.GetNames<DisplayColor>();
            foreach (var t in s)
            {
                builder.Append("[");
                builder.Append(t);
                builder.Append("], ");
            }
            return builder.ToString();
        }

        public bool HandleColorInput(Aisling aisling, Dialog dialog, int? option = null)
        {
            if (!dialog.MenuArgs.TryGet<DisplayColor>(1, out var color))
            {
                dialog.Reply(aisling, DialogString.UnknownInput.Value);
                return false;
            }
            displayColor = color;
            return true;
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (item == null)
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var itemname))
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);
                    return;
                }
                item = source.Inventory.FirstOrDefault(i => i.DisplayName.EqualsI(itemname));
                if (item == null)
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);
                    return;
                }
            }
            inputCollector.Collect(source, Subject, optionIndex);
        }

        public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
        {
            if (option is not 1)
            {
                return false;
            }
            aisling.Inventory.Update(item!.Slot, i =>
            {

                i.Color = displayColor!.Value;

            });
            dialog.Close(aisling);
            return true;
        }
    }
}
