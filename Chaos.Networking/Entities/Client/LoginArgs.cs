using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Login" /> packet
/// </summary>
/// <param name="Name">The name of the aisling the client is trying to log in as</param>
/// <param name="Password">The password of the aisling the client is trying to log in as</param>
public sealed record LoginArgs(string Name, string Password) : IReceiveArgs;