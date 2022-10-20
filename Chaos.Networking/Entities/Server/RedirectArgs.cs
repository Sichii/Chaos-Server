using Chaos.Networking.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record RedirectArgs(IRedirect Redirect) : ISendArgs;