using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record RedirectArgs(Redirect Redirect) : ISendArgs;