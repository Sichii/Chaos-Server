using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record MetafileArgs : ISendArgs
{
    public ICollection<MetafileInfo>? Info { get; set; }
    public MetafileInfo? MetafileData { get; set; }
    public MetafileRequestType MetafileRequestType { get; set; }
}