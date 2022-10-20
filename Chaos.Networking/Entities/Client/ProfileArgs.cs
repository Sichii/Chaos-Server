using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ProfileArgs(byte[] PortraitData, string ProfileMessage) : IReceiveArgs;