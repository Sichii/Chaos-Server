using Chaos.Networking.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Redirect" />
///     packet
/// </summary>
public sealed record RedirectArgs(IRedirect Redirect) : ISendArgs;