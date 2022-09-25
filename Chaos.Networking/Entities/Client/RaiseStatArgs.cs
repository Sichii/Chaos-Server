using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record RaiseStatArgs(Stat Stat) : IReceiveArgs;