using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record BoardRequestArgs(BoardRequestType BoardRequestType) : IReceiveArgs;