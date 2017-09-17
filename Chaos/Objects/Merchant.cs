using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal class Merchant : Creature
    {
        internal DateTime LastClicked { get; set; }
        internal bool ShouldDisplay => DateTime.UtcNow.Subtract(LastClicked).TotalMilliseconds < 500;
        private List<PursuitIds> AvailablePursuits { get; }
        internal Menu Menu { get; }

        private Dictionary<PursuitIds, Pursuit> AllPursuits = new Dictionary<PursuitIds, Pursuit>()
        {
            { PursuitIds.None, new Pursuit("None", PursuitIds.None, 0) },
            { PursuitIds.Revive, new Pursuit("Revive", PursuitIds.Revive, 0) }
        };

        internal Merchant(string name, ushort sprite, CreatureType type, Point point, Map map, Direction direction, List<PursuitIds> availablePursuits, MenuType menuType = MenuType.Menu, string menuText = "What would you like to do?")
            : base(name, (ushort)(sprite + CONSTANTS.MERCHANT_SPRITE_OFFSET), type, point, map, direction)
        {
            LastClicked = DateTime.MinValue;
            AvailablePursuits = availablePursuits;
            Menu = new Menu(AllPursuits.Where(kvp => AvailablePursuits.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList(), menuType, menuText);
        }
    }
}
