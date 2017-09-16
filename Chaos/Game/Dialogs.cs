using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    internal delegate void DialogDelegate(Client client, Server server, Dialog dialog);
    internal sealed class Dialogs
    {
        internal Dialog this[ushort dialogId] => DialogList[dialogId];
        private Dictionary<ushort, DialogDelegate> DialogEffects { get; }
        private Dictionary<ushort, Dialog> DialogList { get; }

        internal Dialogs()
        {
            //these will use pursuit id to get the effect
            DialogEffects = new Dictionary<ushort, DialogDelegate>()
            {
                { 1, new DialogDelegate(Revive) }
            };

            //these will use dialog id to get the dialog
            DialogList = new Dictionary<ushort, Dialog>()
            {
                {
                    1, new Dialog(MenuType.Menu, 0, 1, false, false, "What would you like to do?",
                    new SortedDictionary<string, ushort>()
                    {
                        { "Teleport", 2 },
                        { "Teleport All", 3 }
                    })
                }
            };
        }

        internal Dialog PreviousDialog(Dialog dialog) => DialogList.Values.FirstOrDefault(d => d.NextDialogId == dialog.Id || d.Options.Values.Contains(dialog.Id));
        internal Dialog NextDialog(ushort dialogId) => DialogList[dialogId];

        internal void Revive(Client client, Server server, Dialog dialog)
        {
            client.User.Attributes.CurrentHP = client.User.Attributes.MaximumHP;
            client.User.Attributes.CurrentMP = client.User.Attributes.MaximumMP;
            client.User.IsAlive = true;
            Game.World.Refresh(client, true);

        }
    }
}
