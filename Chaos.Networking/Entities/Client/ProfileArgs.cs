using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record ProfileArgs(byte[] PortraitData, string ProfileMessage) : IReceiveArgs;