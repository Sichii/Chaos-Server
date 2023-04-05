using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.CreateCharRequest" /> packet
/// </summary>
/// <param name="Name">The name of the character</param>
/// <param name="Password">The password of the character</param>
public sealed record CreateCharRequestArgs(string Name, string Password) : IReceiveArgs;