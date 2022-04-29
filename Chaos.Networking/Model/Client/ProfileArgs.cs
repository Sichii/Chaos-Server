using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ProfileArgs(byte[] PortraitData, string ProfileMessage) : IReceiveArgs;