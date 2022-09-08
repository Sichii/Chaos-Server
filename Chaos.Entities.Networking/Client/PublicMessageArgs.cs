using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record PublicMessageArgs(PublicMessageType PublicMessageType, string Message) : IReceiveArgs;