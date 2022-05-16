using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record MetafileArgs : ISendArgs
{
    public ICollection<MetafileDataArg>? Info { get; set; }
    public MetafileDataArg? MetafileData { get; set; }
    public MetafileRequestType MetafileRequestType { get; set; }
}