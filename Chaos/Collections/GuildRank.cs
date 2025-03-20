#region
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a rank within a guild
/// </summary>
public sealed class GuildRank
{
    /// <summary>
    ///     The names of the members of this rank
    /// </summary>
    private readonly HashSet<string> MemberNames;

    /// <summary>
    ///     The name of the rank
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     The tier of the rank. Lower is better
    /// </summary>
    public int Tier { get; }

    public bool CanBeDemoted => Tier < 3;

    public bool CanBePromoted => Tier > 0;

    /// <summary>
    ///     The number of members in this rank
    /// </summary>
    public int Count => MemberNames.Count;

    public bool IsLeaderRank => Tier == 0;

    public bool IsOfficerRank => Tier <= 1;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GuildRank" /> class
    /// </summary>
    /// <param name="name">
    ///     The name of the rank
    /// </param>
    /// <param name="tier">
    ///     The tier of the rank. Lower is better
    /// </param>
    /// <param name="memberNames">
    ///     The members to populate the rank with
    /// </param>
    public GuildRank(string name, int tier, ICollection<string>? memberNames = null)
    {
        memberNames ??= Array.Empty<string>();

        Name = name;
        Tier = tier;
        MemberNames = new HashSet<string>(memberNames, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Adds a member to the rank
    /// </summary>
    /// <param name="memberName">
    ///     The name of the member to add
    /// </param>
    public void AddMember(string memberName) => MemberNames.Add(memberName);

    /// <summary>
    ///     Changes the name of the rank
    /// </summary>
    /// <param name="newName">
    ///     The new name of the rank
    /// </param>
    /// <param name="clientRegistry">
    ///     A registry of logged in aislings in the world
    /// </param>
    public void ChangeRankName(string newName, IEnumerable<IChaosWorldClient> clientRegistry)
    {
        Name = newName;

        foreach (var member in GetOnlineMembers(clientRegistry))
            member.Client.SendSelfProfile();
    }

    /// <summary>
    ///     Gets the names of the members of this rank
    /// </summary>
    public IEnumerable<string> GetMemberNames() => MemberNames;

    /// <summary>
    ///     Gets the online members of this rank
    /// </summary>
    /// <param name="clientRegistry">
    ///     A registry of logged in aislings in the world
    /// </param>
    public IEnumerable<Aisling> GetOnlineMembers(IEnumerable<IChaosWorldClient> clientRegistry)
        => clientRegistry.Where(cli => MemberNames.Contains(cli.Aisling.Name))
                         .Select(cli => cli.Aisling);

    /// <summary>
    ///     Determines if the specified name is a member of this rank
    /// </summary>
    /// <param name="memberName">
    ///     The name to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the name is a member of this rank, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool HasMember(string memberName) => MemberNames.Contains(memberName);

    public bool IsInferiorTo(GuildRank other, int minDiff = 1) => (other.Tier - Tier) >= minDiff;

    public bool IsSuperiorTo(GuildRank other, int minDiff = 1) => (Tier - other.Tier) >= minDiff;

    /// <summary>
    ///     Removes a member from the rank
    /// </summary>
    /// <param name="memberName">
    ///     The name of the member to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the member was found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool RemoveMember(string memberName) => MemberNames.Remove(memberName);
}