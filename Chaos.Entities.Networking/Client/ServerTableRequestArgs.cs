using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record ServerTableRequestArgs(ServerTableRequestType ServerTableRequestType, byte? ServerId) : IReceiveArgs;