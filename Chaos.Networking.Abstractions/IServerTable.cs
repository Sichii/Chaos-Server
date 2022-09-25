namespace Chaos.Networking.Abstractions;

public interface IServerTable
{
    uint CheckSum { get; }
    byte[] Data { get; }
    Dictionary<byte, IServerInfo> Servers { get; }
}