using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ServerTableRequestArgs(ServerTableRequestType ServerTableRequestType, byte? ServerId) : IReceiveArgs;