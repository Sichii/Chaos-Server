using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ClientException" />
///     packet
/// </summary>
/// <param name="ExceptionStr"></param>
public sealed record ClientExceptionArgs(string ExceptionStr) : IReceiveArgs;