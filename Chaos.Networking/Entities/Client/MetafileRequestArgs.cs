using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record MetafileRequestArgs(MetafileRequestType MetafileRequestType, string? Name = null) : IReceiveArgs;