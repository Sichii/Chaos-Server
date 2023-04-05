using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.SocialStatus" /> packet <br />
/// </summary>
/// <param name="SocialStatus">The social status the client is trying to change their status to</param>
public sealed record SocialStatusArgs(SocialStatus SocialStatus) : IReceiveArgs;