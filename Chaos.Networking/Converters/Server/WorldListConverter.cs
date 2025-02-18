#region
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="WorldListArgs" />
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
            var infoByte = (byte)user.BaseClass;

            if (user.IsGuilded)
                infoByte |= 0b_0000_1000;

            //this isnt used in the current UI
            /*if(user.IsWithinLevelRange)
                infoByte |= 0b_0001_0000;*/

            //there are other bit flags here but i dont know what they do or if they are used by anything

            writer.WriteByte(infoByte);
            writer.WriteByte((byte)user.Color);
            writer.WriteByte((byte)user.SocialStatus);
            writer.WriteString8(user.Title ?? string.Empty);
            writer.WriteBoolean(user.IsMaster);
            writer.WriteString8(user.Name);
        }
    }
}