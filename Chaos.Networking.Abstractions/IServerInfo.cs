namespace Chaos.Networking.Abstractions;

public interface IServerInfo : IRedirectInfo
{
    string Description { get; set; }
    byte Id { get; set; }
    string Name { get; set; }
}