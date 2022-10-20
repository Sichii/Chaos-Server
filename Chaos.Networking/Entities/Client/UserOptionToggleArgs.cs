using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record UserOptionToggleArgs(UserOption UserOption) : IReceiveArgs;