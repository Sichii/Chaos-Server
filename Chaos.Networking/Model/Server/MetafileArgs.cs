using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record MetafileArgs : ISendArgs
{
    public ICollection<MetafileInfo>? Info { get; set; }
    public MetafileInfo? MetafileData { get; set; }
    public MetafileRequestType MetafileRequestType { get; set; }
}