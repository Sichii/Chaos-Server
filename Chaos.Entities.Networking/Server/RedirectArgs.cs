using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record RedirectArgs(Redirect Redirect) : ISendArgs;