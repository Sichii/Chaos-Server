using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.PasswordChange" /> packet
/// </summary>
/// <param name="Name">The name of the character for which the client is trying to change the password</param>
/// <param name="CurrentPassword">The current password of the character</param>
/// <param name="NewPassword">The intended new password for the character</param>
public sealed record PasswordChangeArgs(string Name, string CurrentPassword, string NewPassword) : IReceiveArgs;