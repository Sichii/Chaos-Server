using Chaos.Core.Geometry;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record PickupArgs(byte DestinationSlot, Point SourcePoint) : IReceiveArgs;