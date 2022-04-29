using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record PursuitRequestArgs(GameObjectType GameObjectType, uint ObjectId, ushort PursuitId, params string[]? Args) : IReceiveArgs;