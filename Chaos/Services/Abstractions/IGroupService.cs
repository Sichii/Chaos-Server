using Chaos.Objects.World;

namespace Chaos.Services.Abstractions;

public interface IGroupService
{
    void AcceptInvite(Aisling sender, Aisling receiver);
    void Invite(Aisling sender, Aisling receiver);
}