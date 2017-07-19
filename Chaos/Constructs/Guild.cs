using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class Guild : IEnumerable
    {
        public IEnumerator GetEnumerator() => Members.GetEnumerator();
        [JsonProperty]
        internal string Name { get; set; }
        [JsonProperty]
        internal Bank Bank { get; set; }
        [JsonProperty]
        internal ConcurrentDictionary<string, string> Members { get; set; } //name, rank
        [JsonProperty]
        internal List<string> Ranks { get; set; }
        /// <summary>
        /// Used to retreive or change the rank of a member.
        /// </summary>
        /// <param name="name">Name of the member who's rank yould like to change/retreive.</param>
        /// <returns></returns>
        internal string this[string name]
        {
            get { return Members.ContainsKey(name) ? Members[name] : null; }
            private set { Members.AddOrUpdate(name, value, (key, oldValue) => value); }
        }

        /// <summary>
        /// Object representing the serverside concept of a Guild.
        /// </summary>
        /// <param name="name">Name of the guild.</param>
        /// <param name="founders">Founding members of the guild.</param>
        internal Guild(string name, List<Objects.User> founders)
        {
            Name = name;
            Bank = new Bank();
            Members = new ConcurrentDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            Ranks = new List<string>()
            {
                "Initiate", "Member", "Council", "Leader"
            };


            foreach (Objects.User member in founders)
                TryAddMember(member);
        }

        [JsonConstructor]
        internal Guild(string name, Bank bank, ConcurrentDictionary<string, string> members, List<string> ranks)
        {
            Name = name;
            Bank = bank;
            Members = members;
            Ranks = ranks;
        }
        /// <summary>
        /// Returns the title of user(name)
        /// </summary>
        /// <param name="name">Name of the user you want the title of.</param>
        internal string TitleOf(string name) => this[name];
        /// <summary>
        /// Attempts to add a member to the guild.
        /// </summary>
        /// <param name="user">User you wish added to the guild.</param>
        internal bool TryAddMember(Objects.User user)
        {
            if (user.Guild != null)
                return false;

            return Members.TryAdd(user.Name, Ranks[0]);
        }
        /// <summary>
        /// Attempts to remove a member from the guild.
        /// </summary>
        /// <param name="user">User you wish removed from the guild.</param>
        internal bool TryRemoveMember(Objects.User user)
        {
            if (user.Guild == null)
                return false;

            string s = string.Empty;
            return Members.TryRemove(user.Name, out s);
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
