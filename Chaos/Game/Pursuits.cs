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

using System.Collections.Generic;
using System.Linq;

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
            { PursuitIds.BecomeWarrior, new PursuitDelegate(BecomeWarrior) },
            { PursuitIds.BecomeWizard, new PursuitDelegate(BecomeWizard) },
            { PursuitIds.BecomePriest, new PursuitDelegate(BecomePriest) },
            { PursuitIds.BecomeMonk, new PursuitDelegate(BecomeMonk) },
            { PursuitIds.BecomeRogue, new PursuitDelegate(BecomeRogue) },
            { PursuitIds.GiveTatteredRobe, new PursuitDelegate(GiveTatteredRobe) },
        };

        internal static PursuitDelegate Activate(PursuitIds pid) => PursuitList[pid];

        #region PursuitEffects
        private static void None(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null) { }

        private static void GiveTatteredRobe(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            /*
            if (!client.User.HasFlag(Quest.MaribelRobes) && client.User.Gender == Gender.Male)
            {
                Game.CreationEngine.GiveItem(client, server, "Male Tattered Robes", 1);
                client.User.AddFlag(Quest.MaribelRobes);
            }
            else if (!client.User.HasFlag(Quest.MaribelRobes) && client.User.Gender == Gender.Female)
            {
                Game.CreationEngine.GiveItem(client, server, "Female Tattered Robes", 1);
                client.User.AddFlag(Quest.MaribelRobes);
            }
            else
                client.SendServerMessage(ServerMessageType.AdminMessage, @"I've already given you robes.");
                */

            if(!client.User.HasFlag(Quest.MaribelRobes))
            {
                Item item = Game.CreationEngine.CreateItem($@"{(client.User.Gender == Gender.Female ? "Female" : "Male")} Tattered Robes");

                if (client.User.Inventory.AddToNextSlot(item))
                {
                    client.User.AddFlag(Quest.MaribelRobes);
                    client.Enqueue(ServerPackets.AddItem(item));
                }
            }
            else
            {

            }
        }

        private static void ReviveSelf(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            Game.Extensions.ReviveUser(client.User);
        }

        private static void ReviveUser(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            User user;

            if (server.TryGetUser(userInput, out user))
                Game.Extensions.ReviveUser(user);
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");
        }

        private static void Teleport(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            Location warpLoc = new Location();
            User user;

            if (Location.TryParse(userInput, out warpLoc))
                Game.Extensions.WarpObj(client.User, new Warp(client.User.Location, warpLoc));
            else if (server.TryGetUser(userInput, out user))
                Game.Extensions.WarpObj(client.User, new Warp(client.User.Location, user.Location));
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid format. ""mapId xCord yCord"" or ""characterName""");
        }

        private static void SummonUser(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            User user;

            if (server.TryGetUser(userInput, out user))
                Game.Extensions.WarpObj(user, new Warp(user.Location, client.User.Location));
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");
        }

        private static void SummonAll(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            IEnumerable<User> allUsers = server.WorldClients.Where(c => c.User != client.User).Select(c => c.User);

            foreach (User user in allUsers)
                Game.Extensions.WarpObj(user, new Warp(user.Location, client.User.Location));
        }

        private static void KillUser(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            User user;

            if (server.TryGetUser(userInput, out user))
                Game.Extensions.ApplyDamage(user, int.MaxValue, true);
            else
                client.SendServerMessage(ServerMessageType.Whisper, @"Invalid name.");
        }

        private static void LouresCitizenship(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            client.User.Nation = Nation.Loures;
        }

        private static void BecomeWarrior(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            client.User.BaseClass = BaseClass.Warrior;
            ClearSpellsSkills(client);

            client.User.SkillBook.AddToNextSlot(Game.CreationEngine.CreateSkill("Cleave"));
            client.User.SkillBook.AddToNextSlot(Game.CreationEngine.CreateSkill("Reposition"));
            client.User.SkillBook.AddToNextSlot(Game.CreationEngine.CreateSkill("Shoulder Charge"));
            //add more skills


            foreach (Skill skill in client.User.SkillBook.Where(s => s != null))
                client.Enqueue(ServerPackets.AddSkill(skill));

            client.Enqueue(ServerPackets.DisplayDialog(client.ActiveObject, Game.Dialogs.CloseDialog()));
        }

        private static void BecomeWizard(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            client.User.BaseClass = BaseClass.Wizard;
            ClearSpellsSkills(client);

            client.Enqueue(ServerPackets.DisplayDialog(client.ActiveObject, Game.Dialogs.CloseDialog()));
        }

        private static void BecomePriest(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            client.User.BaseClass = BaseClass.Priest;
            ClearSpellsSkills(client);

            client.Enqueue(ServerPackets.DisplayDialog(client.ActiveObject, Game.Dialogs.CloseDialog()));
        }

        private static void BecomeMonk(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            client.User.BaseClass = BaseClass.Monk;
            ClearSpellsSkills(client);

            client.Enqueue(ServerPackets.DisplayDialog(client.ActiveObject, Game.Dialogs.CloseDialog()));
        }

        private static void BecomeRogue(Client client, Server server, bool closing = false, byte menuOption = 0, string userInput = null)
        {
            client.User.BaseClass = BaseClass.Rogue;
            ClearSpellsSkills(client);

            client.Enqueue(ServerPackets.DisplayDialog(client.ActiveObject, Game.Dialogs.CloseDialog()));
        }
        #endregion


        #region Modular Methods
        private static void ClearSpellsSkills(Client client)
        {
            foreach (Skill skill in client.User.SkillBook.Where(s => s != null).ToList())
                if (client.User.SkillBook.TryRemove(skill.Slot))
                    client.Enqueue(ServerPackets.RemoveSkill(skill.Slot));

            foreach (Spell spell in client.User.SpellBook.Where(s => s != null).ToList())
                if (spell.Name != "Admin Create" && client.User.SpellBook.TryRemove(spell.Slot))
                    client.Enqueue(ServerPackets.RemoveSpell(spell.Slot));
        }
        #endregion
    }
}
