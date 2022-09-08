using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record MetafileArgs : ISendArgs
{
    public ICollection<MetafileInfo>? Info { get; set; }
    public MetafileInfo? MetafileData { get; set; }
    public MetafileRequestType MetafileRequestType { get; set; }
}