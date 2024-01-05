using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="WorldListArgs" /> into a buffer
/// </summary>
public sealed class WorldListConverter : PacketConverterBase<WorldListArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.WorldList;

    /// <inheritdoc />
    public override WorldListArgs Deserialize(ref SpanReader reader)
    {
        var worldMemberCount = reader.ReadUInt16();
        var countryCount = reader.ReadUInt16();
        var countryList = new List<WorldListMemberInfo>(countryCount);

        for (var i = 0; i < countryCount; i++)
        {
            var baseClass = reader.ReadByte();
            var color = reader.ReadByte();
            var socialStatus = reader.ReadByte();
            var title = reader.ReadString8();
            var isMaster = reader.ReadBoolean();
            var name = reader.ReadString8();

            countryList.Add(
                new WorldListMemberInfo
                {
                    BaseClass = (BaseClass)baseClass,
                    Color = (WorldListColor)color,
                    SocialStatus = (SocialStatus)socialStatus,
                    Title = title,
                    IsMaster = isMaster,
                    Name = name
                });
        }

        return new WorldListArgs
        {
            WorldMemberCount = worldMemberCount,
            CountryList = countryList
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, WorldListArgs args)
    {
        writer.WriteUInt16(args.WorldMemberCount);
        writer.WriteUInt16((ushort)args.CountryList.Count);

        foreach (var user in args.CountryList)
        {
            writer.WriteByte((byte)user.BaseClass);
            writer.WriteByte((byte)user.Color);
            writer.WriteByte((byte)user.SocialStatus);
            writer.WriteString8(user.Title ?? string.Empty);
            writer.WriteBoolean(user.IsMaster);
            writer.WriteString8(user.Name);
        }
    }
}