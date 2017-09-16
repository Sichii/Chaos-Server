using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal class Merchant : Creature
    {
        internal DateTime LastClicked { get; set; }
        internal bool ShouldDisplay => DateTime.UtcNow.Subtract(LastClicked).TotalMilliseconds < 500;
        internal Dictionary<ushort, Dialog> Dialogs { get; set; } //pursuitid > dialog
        internal List<ushort> AvailablePursuits { get; set; }
        internal MenuType MenuType { get; set; }
        internal Menu Menu
        {
            get
            {
                //get all pursuits that have pursuit ids contained within availablepursuits for this merchant
                List<Pursuit> pursuits = AllPursuits.Where(kvp => AvailablePursuits.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList();
                //get all dialogs where a pursuit
                List<Dialog> dialogs = Dialogs.Where(kvp => pursuits.Select(p => p.DialogId).Contains(kvp.Key)).Select(kvp => kvp.Value).ToList();

                return new Menu(pursuits, dialogs, MenuType);
            }

        }
        private Dictionary<ushort, Pursuit> AllPursuits = new Dictionary<ushort, Pursuit>()
        {
            { 1, new Pursuit("Revive", 1, 0) }
        };

        internal Merchant(string name, ushort sprite, CreatureType type, Point point, Map map, Direction direction, List<ushort> availablePursuits, List<ushort> dialogIds, MenuType menuType = MenuType.Menu)
            : base(name, (ushort)(sprite + Game.MERCHANT_SPRITE_OFFSET), type, point, map, direction)
        {
            LastClicked = DateTime.MinValue;
            Dialogs = new Dictionary<ushort, Dialog>();
            AvailablePursuits = availablePursuits;
            MenuType = menuType;

            Dialogs.Add(0, default(Dialog)); //this will represent that there's no following dialog

            foreach (ushort did in dialogIds)
                Dialogs.Add(did, Game.Dialogs[did]);
        }
    }
}
