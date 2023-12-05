namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents the notice shown when logging into a server
/// </summary>
public interface INotice
{
    /// <summary>
    ///     A value that can be used to quickly verify the contents of the notice
    /// </summary>
    uint CheckSum { get; }

    /// <summary>
    ///     The notice's message in compressed form
    /// </summary>
    byte[] Data { get; }
}