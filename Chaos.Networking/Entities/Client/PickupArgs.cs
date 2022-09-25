using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record PickupArgs(byte DestinationSlot, IPoint SourcePoint) : IReceiveArgs;