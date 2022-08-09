using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record MetafileRequestArgs(MetafileRequestType MetafileRequestType, string? Name = null) : IReceiveArgs;