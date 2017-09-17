using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Guild : IEnumerable
    {
        private static readonly object Sync = new object();
        public IEnumerator GetEnumerator() => Members.GetEnumerator();
        [JsonProperty]
        internal string Name { get; set; }
        [JsonProperty]
        private Bank Bank { get; set; }
        [JsonProperty]
        private Dictionary<string, string> Members; //name, rank
        [JsonProperty]
        private List<string> Ranks { get; set; }

        internal Guild()
        {
            Name = "Chaos Team";
            Bank = new Bank();
            Ranks = new List<string>()
            {
                "Event Host", "Game Master", "Developer", "Lead Developer"
            };
            Members = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            Members.Add(Server.Admins[0], Ranks[3]);
            Members.Add(Server.Admins[1], Ranks[2]);
            Members.Add(Server.Admins[2], Ranks[2]);
        }

        /// <summary>
        /// Object representing the serverside concept of a Guild.
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

        [JsonConstructor]
        internal Guild(string name, Bank bank, Dictionary<string, string> members, List<string> ranks)
        {
            Name = name;
            Bank = bank;
            Members = new Dictionary<string, string>(members, StringComparer.CurrentCultureIgnoreCase);
            Ranks = ranks;
        }
        /// <summary>
        /// Returns the title of user(name)
        /// </summary>
        /// <param name="name">Name of the user you want the title of.</param>
        internal string TitleOf(string name) => Members.ContainsKey(name) ? Members[name] : null;

        /// <summary>
        /// Attempts to add a member to the guild.
        /// </summary>
        /// <param name="user">User you wish added to the guild.</param>
        internal bool TryAddMember(User user)
        {
            if (user.Guild != null)
                return false;

            Members.Add(user.Name, Ranks[0]);
            user.Guild = this;

            return Members.ContainsKey(user.Name);
        }
        /// <summary>
        /// Attempts to remove a member from the guild.
        /// </summary>
        /// <param name="user">User you wish removed from the guild.</param>
        internal bool TryRemoveMember(User user)
        {
            if (user.Guild == null)
                return false;

            user.Guild = null;
            return Members.Remove(user.Name);
        }
        /// <summary>
        /// Attempts to change the name of a rank.
        /// </summary>
        /// <param name="oldRank">Name of the old rank.</param>
        /// <param name="newRank">New name of the rank.</param>
        internal bool TryChangeRankName(string oldRank, string newRank)
        {
            if (!string.IsNullOrEmpty(oldRank) && !string.IsNullOrEmpty(newRank) && Ranks.Contains(oldRank))
            {
                Ranks[Ranks.IndexOf(oldRank)] = newRank;
                return true;
            }
            return false;
        }
    }
}
