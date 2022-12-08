using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not implemented")]
public sealed record BoardRequestArgs(BoardRequestType BoardRequestType) : IReceiveArgs;