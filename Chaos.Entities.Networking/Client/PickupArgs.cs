using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record PickupArgs(byte DestinationSlot, IPoint SourcePoint) : IReceiveArgs;