using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record MetafileRequestArgs(MetafileRequestType MetafileRequestType, string? Name = null) : IReceiveArgs;