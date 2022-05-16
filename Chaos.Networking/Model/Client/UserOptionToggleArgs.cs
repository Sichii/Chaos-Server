using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record UserOptionToggleArgs(UserOption UserOption) : IReceiveArgs;