// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal class Merchant : Creature
    {
        internal DateTime LastClicked { get; set; }
        internal bool ShouldDisplay => DateTime.UtcNow.Subtract(LastClicked).TotalMilliseconds < 500;
        internal ushort NextDialogId { get; }
        private List<PursuitIds> AvailablePursuits { get; }
        internal Menu Menu { get; }
        internal override byte HealthPercent => 100;
        internal override uint CurrentHP { get { return uint.MaxValue; } set { } }
        internal override uint MaximumHP { get { return uint.MaxValue; } }

        private Dictionary<PursuitIds, Pursuit> AllPursuits = new Dictionary<PursuitIds, Pursuit>()
        {
            { PursuitIds.None, new Pursuit("None", PursuitIds.None, 0) },
            { PursuitIds.Revive, new Pursuit("Revive", PursuitIds.Revive, 0) },
            { PursuitIds.Teleport, new Pursuit("Teleport", PursuitIds.Teleport, 2) },
            { PursuitIds.Summon, new Pursuit("Summon", PursuitIds.Summon, 3) },
            { PursuitIds.SummonAll, new Pursuit("Summon All", PursuitIds.SummonAll, 4) },
            { PursuitIds.KillUser, new Pursuit("Kill User", PursuitIds.KillUser, 5) },
            { PursuitIds.LouresCitizenship, new Pursuit("Citizenship", PursuitIds.LouresCitizenship, 6) },
            { PursuitIds.ReviveUser, new Pursuit("Revive User", PursuitIds.ReviveUser, 8) },
        };

        internal Merchant(string name, ushort sprite, CreatureType type, Point point, Map map, Direction direction, ushort nextDialogId = 0, List<PursuitIds> availablePursuits = null, MenuType menuType = MenuType.Menu, string menuText = "What would you like to do?")
            : base(name, (ushort)(sprite + CONSTANTS.MERCHANT_SPRITE_OFFSET), type, point, map, direction)
        {
            NextDialogId = nextDialogId;
            LastClicked = DateTime.MinValue;
            AvailablePursuits = availablePursuits;
            Menu = new Menu(AllPursuits.Where(kvp => AvailablePursuits.Contains(kvp.Key)).Select(kvp => kvp.Value), menuType, menuText);
        }
    }
}
