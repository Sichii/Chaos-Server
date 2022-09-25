using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record PasswordChangeArgs(string Name, string CurrentPassword, string NewPassword) : IReceiveArgs;