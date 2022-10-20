using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record BoardRequestArgs(BoardRequestType BoardRequestType) : IReceiveArgs;