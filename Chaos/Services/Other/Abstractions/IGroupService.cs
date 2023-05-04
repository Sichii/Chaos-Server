using Chaos.Models.World;

namespace Chaos.Services.Other.Abstractions;

public interface IGroupService
{
    void AcceptInvite(Aisling sender, Aisling receiver);
    void Invite(Aisling sender, Aisling receiver);
}