using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record PasswordChangeArgs(string Name, string CurrentPassword, string NewPassword) : IReceiveArgs;