using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record PickupArgs(byte DestinationSlot, IPoint SourcePoint) : IReceiveArgs;