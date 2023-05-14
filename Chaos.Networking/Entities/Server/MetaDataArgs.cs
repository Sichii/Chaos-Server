using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.MetaData" />
///     packet
/// </summary>
public sealed record MetaDataArgs : ISendArgs
{
    /// <summary>
    ///     If this request type is to validate checksums of all meta files, this is the collection of meta file details to
    ///     validate
    /// </summary>
    public ICollection<MetaDataInfo>? Info { get; set; }
    /// <summary>
    ///     If this request type is to give raw data of a meta file, this is the meta file data to send
    /// </summary>
    public MetaDataInfo? MetaDataData { get; set; }
    /// <summary>
    ///     The type of the meta file request
    /// </summary>
    public MetaDataRequestType MetaDataRequestType { get; set; }
}