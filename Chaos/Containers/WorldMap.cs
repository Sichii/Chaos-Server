using System.IO;
using Chaos.Data;
using Chaos.Extensions.Cryptography;

namespace Chaos.Containers;

public sealed class WorldMap
{
    public required string WorldMapKey { get; init; }
    public required byte FieldIndex { get; init; }
    public required Dictionary<ushort, WorldMapNode> Nodes { get; init; } = new();


    /*
/// <summary>
///     Gets the checksum of the worldmap, based on it's nodes.
/// </summary>
/// <returns></returns>
public uint CheckSum
{
    get
    {
        var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream);

        binaryWriter.Write((byte)Nodes.Count);

        foreach (var worldMapNode in Nodes)
        {
            binaryWriter.Write(worldMapNode.ScreenPosition.X);
            binaryWriter.Write(worldMapNode.ScreenPosition.Y);
            binaryWriter.Write(worldMapNode.Text);
            binaryWriter.Write(worldMapNode.DestinationMapId);
        }

        binaryWriter.Flush();

        return memoryStream.ToArray().Generate32();
    }
}
*/
}