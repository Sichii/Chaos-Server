using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Click" /> packet
/// </summary>
/// <param name="TargetId">If specified, the id of the object being clicked on</param>
/// <param name="TargetPoint">If specified, the point being clicked on</param>
public sealed record ClickArgs(uint? TargetId, IPoint? TargetPoint) : IReceiveArgs;