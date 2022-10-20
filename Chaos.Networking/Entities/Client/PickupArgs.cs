using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record PickupArgs(byte DestinationSlot, IPoint SourcePoint) : IReceiveArgs;