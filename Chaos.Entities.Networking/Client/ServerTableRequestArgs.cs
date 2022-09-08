using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ServerTableRequestArgs(ServerTableRequestType ServerTableRequestType, byte? ServerId) : IReceiveArgs;