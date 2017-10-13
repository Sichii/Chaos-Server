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

namespace Chaos
{
    internal class Extensions
    {
        private Server Server { get; set; }
        private World World { get; set; }

        internal Extensions(Server server, World world)
        {
            Server = server;
            World = world;
        }

        internal void KillUser(Client client, User user)
        {
            user.Attributes.CurrentHP = 0;
            user.IsAlive = false;
            //strip buffs
            //disable casting
            //death stuff

            World.Refresh(client, true);
        }

        internal void Revive(Client client, Server server, object args)
        {
            if (!client.User.IsAlive)
            {
                client.User.Attributes.CurrentHP = client.User.Attributes.MaximumHP;
                client.User.Attributes.CurrentMP = client.User.Attributes.MaximumMP;
                client.User.IsAlive = true;
                Game.World.Refresh(client, true);
            }
        }

        internal void ReviveUser(Client client, User user)
        {
            user.Attributes.CurrentHP = user.Attributes.MaximumHP;
            user.Attributes.CurrentMP = user.Attributes.MaximumMP;
            user.IsAlive = true;
            Game.World.Refresh(client, true);
        }
    }
}