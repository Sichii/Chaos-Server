using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.EditableProfileRequest" /> packet
/// </summary>
public sealed record EditableProfileRequestArgs : IPacketSerializable;