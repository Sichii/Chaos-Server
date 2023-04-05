using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.PublicMessage" /> packet <br />
/// </summary>
/// <param name="PublicMessageType">The type of the public message the client is trying to send</param>
/// <param name="Message">The message the client is trying to send</param>
public sealed record PublicMessageArgs(PublicMessageType PublicMessageType, string Message) : IReceiveArgs;