using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.MetaData" /> packet
/// </summary>
public sealed record MetaDataArgs : IPacketSerializable
{
    /// <summary>
    ///     If this request type is to validate checksums of all meta files, this is the collection of meta file details to
    ///     validate
    /// </summary>
    public ICollection<MetaDataInfo>? MetaDataCollection { get; set; }

    /// <summary>
    ///     If this request type is to give raw data of a meta file, this is the meta file data to send
    /// </summary>
    public MetaDataInfo? MetaDataInfo { get; set; }

    /// <summary>
    ///     The type of the meta file request
    /// </summary>
    public MetaDataRequestType MetaDataRequestType { get; set; }
}