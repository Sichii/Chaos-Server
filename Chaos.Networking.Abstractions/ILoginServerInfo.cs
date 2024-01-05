namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a configuration object that is used to describe a login server
/// </summary>
public interface ILoginServerInfo : IConnectionInfo
{
    /// <summary>
    ///     A brief description of the server. Must be less than 18 characters.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    ///     A unique id used to represent the server. If the server is external, this Id must be communicated with the owner of
    ///     that server.
    /// </summary>
    byte Id { get; set; }

    /// <summary>
    ///     The name of the server. Must be less than 9 characters. If the server is external, this name must be communicated
    ///     with the owner of that server.
    /// </summary>
    string Name { get; set; }
}