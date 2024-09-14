using Chaos.Models.World;

namespace Chaos.Services.Other.Abstractions;

public interface IGroupService
{
    enum RequestType
    {
        Invite,
        RequestToJoin
    }

    void AcceptInvite(Aisling sender, Aisling receiver);
    void AcceptRequestToJoin(Aisling sender, Aisling receiver);
    RequestType? DetermineRequestType(Aisling sender, Aisling receiver);
    void Invite(Aisling sender, Aisling receiver);
    void RequestToJoin(Aisling sender, Aisling receiver);
}