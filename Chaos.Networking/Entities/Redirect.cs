using System.Net;
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions;

namespace Chaos.Networking.Entities;

/// <inheritdoc />
public sealed record Redirect : IRedirect
{
    /// <inheritdoc />
    public DateTime Created { get; }

    /// <inheritdoc />
    public IPEndPoint EndPoint { get; }

    /// <inheritdoc />
    public uint Id { get; }

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public byte Seed { get; }

    /// <inheritdoc />
    public ServerType Type { get; }

    /// <summary>
    ///     Creates a new <see cref="Redirect" />
    /// </summary>
    /// <param name="id">
    ///     The id of the redirect
    /// </param>
    /// <param name="serverInfo">
    ///     Info about the server being redirected to
    /// </param>
    /// <param name="type">
    ///     The type of redirect
    /// </param>
    /// <param name="key">
    ///     The encryption key to use when joining to the redirected server
    /// </param>
    /// <param name="seed">
    ///     The encryption seed to use when joining to the redirected server
    /// </param>
    /// <param name="name">
    ///     The name associated with the redirect
    /// </param>
    public Redirect(
        uint id,
        IConnectionInfo serverInfo,
        ServerType type,
        string key,
        byte seed,
        string? name = null)
    {
        Id = id;
        Type = type;
        Key = key;
        Seed = seed;
        Name = name ?? "Login";
        Created = DateTime.UtcNow;

        var address = serverInfo.Address;

        if (IPAddress.IsLoopback(address))
            address = IPAddress.Loopback;

        EndPoint = new IPEndPoint(address, serverInfo.Port);
    }
}