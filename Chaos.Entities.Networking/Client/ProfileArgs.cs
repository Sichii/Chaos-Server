using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ProfileArgs(byte[] PortraitData, string ProfileMessage) : IReceiveArgs;