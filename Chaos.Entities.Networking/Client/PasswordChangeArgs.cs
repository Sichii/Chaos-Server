using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record PasswordChangeArgs(string Name, string CurrentPassword, string NewPassword) : IReceiveArgs;