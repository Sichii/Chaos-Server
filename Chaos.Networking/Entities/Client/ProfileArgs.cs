using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Profile" />
///     packet
/// </summary>
/// <param name="PortraitData">The data of the client's custom portrait</param>
/// <param name="ProfileMessage">The text in the client's custom profile</param>
public sealed record ProfileArgs(byte[] PortraitData, string ProfileMessage) : IReceiveArgs;