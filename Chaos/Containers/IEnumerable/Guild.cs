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

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Chaos
{
    internal sealed class Guild : IEnumerable
    {
        private readonly object Sync = new object();
        private readonly Bank Bank;
        private readonly Dictionary<string, string> Members; //name, rank
        private readonly List<string> Ranks;

        internal string Name { get; private set; }

        public IEnumerator GetEnumerator()
        {
            IEnumerator safeEnum = Members.GetEnumerator();

            lock (Sync)
                while (safeEnum.MoveNext())
                    yield return safeEnum.Current;
        }

        /// <summary>
        /// Default constructor for an enumerable object. Represents the serverside concept of a Guild or Clan.
        /// The default constructor is currently the GM guild.
        /// </summary>
        internal Guild()
        {
            Name = CONSTANTS.DEVELOPER_GUILD_NAME;
            Bank = new Bank();
            Ranks = new List<string>()
            {
                "Guinea Pig", "Event Host", "Game Master", "Developer", "Lead Developer"
            };
            Members = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
            {
                { "Sichi", Ranks[4] },
                { "Jinori", Ranks[3] },
                { "Whug", Ranks[3] },
                { "Doms", Ranks[3] },
                { "Vorlof", Ranks[2] },
                { "JohnGato", Ranks[2] },
                { "Pill", Ranks[1] },
                { "Ishikawa", Ranks[0] },
                { "Legend", Ranks[0] },
                { "Styax", Ranks[0] },
            };
        }

        /// <summary>
        /// Base constructor for an enumerable object. Represents the serverside concept of a Guild, or Clan.
        /// </summary>
        /// <param name="name">Name of the guild.</param>
        /// <param name="founders">Founding members of the guild.</param>
        internal Guild(string name, List<User> founders)
        {
            Name = name;
            Bank = new Bank();
            Members = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            Ranks = new List<string>()
            {
                "Initiate", "Member", "Council", "Leader"
            };

            foreach (User member in founders)
                TryAddMember(member);
        }

        /// <summary>
        /// Master constructor for an enumerable object. Represents the serverside concept of a Guild, or Clan. Populated at server load from the database.
        /// </summary>
        internal Guild(string name, Dictionary<string, string> members, List<string> ranks)
        {
            Name = name;
            Members = new Dictionary<string, string>(members, StringComparer.CurrentCultureIgnoreCase);
            Ranks = ranks;
        }

        /// <summary>
        /// Json constructor for guilds. Will use this name to fetch the guild reference from a pre-populated list.
        /// </summary>
        [JsonConstructor]
        internal Guild(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Synchronously returns the title of user.
        /// </summary>
        /// <param name="name">Name of the user you want the title of.</param>
        internal string TitleOf(string name)
        {
            lock (Sync)
                return Members.ContainsKey(name) ? Members[name] : null;
        }

        /// <summary>
        /// Attempts to synchronously add a member to the guild.
        /// </summary>
        /// <param name="user">User you wish added to the guild.</param>
        internal bool TryAddMember(User user)
        {
            lock (Sync)
            {
                if (user.Guild != null || Members.ContainsKey(user.Name))
                    return false;

                Members.Add(user.Name, Ranks[0]);
                user.GuildName = Name;

                return Members.ContainsKey(user.Name) && user.Guild == this;
            }
        }

        /// <summary>
        /// Attempts to synchronously remove a member from the guild.
        /// </summary>
        /// <param name="user">User you wish removed from the guild.</param>
        internal bool TryRemoveMember(User user)
        {
            lock (Sync)
            {
                if (user.Guild == null || !Members.ContainsKey(user.Name))
                    return false;

                user.GuildName = null;
                return Members.Remove(user.Name);
            }
        }

        /// <summary>
        /// Attempts to synchronously change a member's rank.
        /// </summary>
        /// <param name="memberName">Name of the member who's rank is to be changed.</param>
        /// <param name="newRank">The rank to change the given member to.</param>
        /// <returns></returns>
        internal bool TryChangeRank(string memberName, string newRank)
        {
            lock(Sync)
            {
                if (Members.ContainsKey(memberName) && Ranks.Contains(newRank))
                {
                    Members[memberName] = Ranks[Ranks.IndexOf(newRank)]; //to make sure we get the capitolization that's in the rank list, rather than what was provided
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Attempts to synchronously change the name of a rank.
        /// </summary>
        /// <param name="oldRank">Name of the old rank.</param>
        /// <param name="newRank">New name of the rank.</param>
        internal bool TryChangeRankName(string oldRank, string newRank)
        {
            lock (Sync)
            {
                if (!string.IsNullOrEmpty(oldRank) && !string.IsNullOrEmpty(newRank) && Ranks.Contains(oldRank))
                {
                    Ranks[Ranks.IndexOf(oldRank)] = newRank;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Synchronously writes guild information to a buffer, to be saved in the database.
        /// </summary>
        /// <param name="writer"></param>
        internal void Save(BinaryWriter writer)
        {
            lock(Sync)
            {
                writer.Write(Name);
                writer.Write(Ranks.Count);

                foreach (string str in Ranks)
                    writer.Write(str);

                writer.Write(Members.Count);

                foreach (KeyValuePair<string, string> kvp in Members)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }

                //write bank?
            }
        }
    }
}
