using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Collections;

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

    public int Count => MemberNames.Count;

    public GuildRank(string name, int tier, ICollection<string>? memberNames = null)
    {
        memberNames ??= Array.Empty<string>();

        Name = name;
        Tier = tier;
        MemberNames = new HashSet<string>(memberNames, StringComparer.OrdinalIgnoreCase);
    }

    public void AddMember(string memberName) => MemberNames.Add(memberName);

    public void ChangeRankName(string newName, IEnumerable<IWorldClient> clientRegistry)
    {
        Name = newName;

        foreach (var member in GetOnlineMembers(clientRegistry))
            member.Client.SendSelfProfile();
    }

    public IEnumerable<string> GetMemberNames() => MemberNames;

    public IEnumerable<Aisling> GetOnlineMembers(IEnumerable<IWorldClient> clientRegistry)
        => clientRegistry.Where(cli => MemberNames.Contains(cli.Aisling.Name))
                         .Select(cli => cli.Aisling);

    public bool HasMember(string memberName) => MemberNames.Contains(memberName);
    public bool RemoveMember(string memberName) => MemberNames.Remove(memberName);
}