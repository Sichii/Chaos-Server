using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record PublicMessageArgs(PublicMessageType PublicMessageType, string Message) : IReceiveArgs;