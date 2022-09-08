using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record PickupArgs(byte DestinationSlot, IPoint SourcePoint) : IReceiveArgs;