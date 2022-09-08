using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record MetafileRequestArgs(MetafileRequestType MetafileRequestType, string? Name = null) : IReceiveArgs;