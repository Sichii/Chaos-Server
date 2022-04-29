using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record LoginArgs(string Name, string Password) : IReceiveArgs;