using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record RedirectArgs(Redirect Redirect) : ISendArgs;