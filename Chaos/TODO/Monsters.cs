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

namespace Chaos
{
    internal sealed class Monsters
    {
        private readonly Dictionary<string, Monster> MonsterDic;

        internal Monsters()
        {
            var monsters = new List<Monster>()
            {
                new Monster("Lich", (8984, 5, 5), 205, CreatureType.Normal, TimeSpan.FromMilliseconds(500), 5000000)
            };
        }
    }
}
