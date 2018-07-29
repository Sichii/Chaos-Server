﻿// ****************************************************************************
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
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    internal delegate void PursuitDelegate(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null);
    internal static class Pursuits
    {
        //these will use pursuit id to get the effect
        private static Dictionary<PursuitIds, PursuitDelegate> PursuitList = new Dictionary<PursuitIds, PursuitDelegate>()
        {
            { PursuitIds.None, new PursuitDelegate(None) },
            { PursuitIds.ReviveSelf, new PursuitDelegate(ReviveSelf) },
            { PursuitIds.ReviveUser, new PursuitDelegate(ReviveUser) },
            { PursuitIds.Teleport, new PursuitDelegate(Teleport) },
            { PursuitIds.SummonUser, new PursuitDelegate(SummonUser) },
            { PursuitIds.SummonAll, new PursuitDelegate(SummonAll) },
            { PursuitIds.KillUser, new PursuitDelegate(KillUser) },
            { PursuitIds.LouresCitizenship, new PursuitDelegate(LouresCitizenship) },
        };

        internal static PursuitDelegate Activate(PursuitIds pid) => PursuitList[pid];

        #region PursuitEffects
        private static void None(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null) { }

        private static void ReviveSelf(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            if(!client.User.IsAlive)
            {
                client.User.Attributes.CurrentHP = client.User.Attributes.MaximumHP;
                client.User.Attributes.CurrentMP = client.User.Attributes.MaximumMP;
                client.User.IsAlive = true;
                Game.World.Refresh(client, true);
            }
        }

        private static void ReviveUser(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            User user;

            if (server.TryGetUser(userInput, out user))
                Game.Extensions.ReviveUser(user.Client, user);
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");
        }

        private static void Teleport(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            Location warpLoc = new Location();
            User user;

            if (Location.TryParse(userInput, out warpLoc))
                Game.World.WarpUser(client.User, new Warp(client.User.Location, warpLoc));
            else if (server.TryGetUser(userInput, out user))
                Game.World.WarpUser(client.User, new Warp(client.User.Location, user.Location));
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid format. ""mapId xCord yCord"" or ""characterName""");
        }

        private static void SummonUser(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            User user;

            if (server.TryGetUser(userInput, out user))
                Game.World.WarpUser(user, new Warp(user.Location, client.User.Location));
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");
        }

        private static void SummonAll(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            IEnumerable<User> allUsers = server.WorldClients.Where(c => c.User != client.User).Select(c => c.User);

            foreach (User user in allUsers)
                Game.World.WarpUser(user, new Warp(user.Location, client.User.Location));
        }

        private static void KillUser(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            User user;

            if (server.TryGetUser(userInput, out user))
                Game.Extensions.KillUser(user.Client, user);
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");
        }

        private static void LouresCitizenship(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            client.User.Nation = Nation.Loures;
        }
        #endregion
    }
}