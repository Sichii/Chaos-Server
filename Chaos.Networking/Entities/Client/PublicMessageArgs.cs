using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record PublicMessageArgs(PublicMessageType PublicMessageType, string Message) : IReceiveArgs;