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
    //will need to figure out how to properly serialize this only a single time
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class Guild : IEnumerable
    {
        private readonly object Sync = new object();
        public IEnumerator GetEnumerator() => Members.GetEnumerator();
        [JsonProperty]
        internal string Name { get; set; }
        private Bank Bank { get; set; }
        private Dictionary<string, string> Members; //name, rank
        private List<string> Ranks { get; set; }

        /// <summary>
        /// Default constructor for an enumerable object. Represents the serverside concept of a Guild or Clan.
        /// The default constructor is currently the GM guild.
        /// </summary>
        internal Guild()
        {
            Name = "Chaos Team";
            Bank = new Bank();
            Ranks = new List<string>()
            {
                "Guinea Pig", "Event Host", "Game Master", "Developer", "Lead Developer"
            };
            Members = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            Members.Add("sichi", Ranks[4]);
            Members.Add("whug", Ranks[0]);
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
                user.Guild = this;

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

                user.Guild = null;
                return Members.Remove(user.Name);
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

                foreach (var kvp in Members)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }

                //write bank?
            }
        }
    }
}
