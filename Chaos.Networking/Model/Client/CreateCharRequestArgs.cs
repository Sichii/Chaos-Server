using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record CreateCharRequestArgs(string Name, string Password) : IReceiveArgs;

//TODO: group box