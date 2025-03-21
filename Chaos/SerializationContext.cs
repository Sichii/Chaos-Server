#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Collections.Time;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Boards;
using Chaos.Schemas.Content;
using Chaos.Schemas.Data;
using Chaos.Schemas.Guilds;
using Chaos.Schemas.MetaData;
using Chaos.Schemas.Templates;
#endregion

// ReSharper disable ArrangeAttributes because it breaks for some reason

namespace Chaos;

//other
[JsonSerializable(typeof(EnumCollection))]
[JsonSerializable(typeof(FlagCollection))]
[JsonSerializable(typeof(CounterCollection))]
[JsonSerializable(typeof(TimedEventCollection))]
[JsonSerializable(typeof(TimedEventCollection.Event))]
[JsonSerializable(typeof(EventMetaSchema))]
[JsonSerializable(typeof(Animation))]

//schemas
[JsonSerializable(typeof(GuildSchema))]
[JsonSerializable(typeof(GuildRankSchema))]
[JsonSerializable(typeof(AislingSchema))]
[JsonSerializable(typeof(ItemRequirementSchema))]
[JsonSerializable(typeof(LearningRequirementsSchema))]
[JsonSerializable(typeof(LootDropSchema))]
[JsonSerializable(typeof(ShardingOptionsSchema))]
[JsonSerializable(typeof(LootTableSchema))]
[JsonSerializable(typeof(MerchantSpawnSchema))]
[JsonSerializable(typeof(MonsterSpawnSchema))]
[JsonSerializable(typeof(ReactorTileSchema))]
[JsonSerializable(typeof(WorldMapNodeSchema))]
[JsonSerializable(typeof(WorldMapSchema))]
[JsonSerializable(typeof(DialogTemplateSchema))]
[JsonSerializable(typeof(ItemTemplateSchema))]
[JsonSerializable(typeof(MapTemplateSchema))]
[JsonSerializable(typeof(MerchantTemplateSchema))]
[JsonSerializable(typeof(MonsterTemplateSchema))]
[JsonSerializable(typeof(ReactorTileTemplateSchema))]
[JsonSerializable(typeof(SkillTemplateSchema))]
[JsonSerializable(typeof(SpellTemplateSchema))]
[JsonSerializable(typeof(DialogOptionSchema))]
[JsonSerializable(typeof(AttributesSchema))]
[JsonSerializable(typeof(BankSchema))]
[JsonSerializable(typeof(EffectSchema))]
[JsonSerializable(typeof(ItemSchema))]
[JsonSerializable(typeof(LegendMarkSchema))]
[JsonSerializable(typeof(MapInstanceSchema))]
[JsonSerializable(typeof(SkillSchema))]
[JsonSerializable(typeof(SpellSchema))]
[JsonSerializable(typeof(StatSheetSchema))]
[JsonSerializable(typeof(StatsSchema))]
[JsonSerializable(typeof(UserOptionsSchema))]
[JsonSerializable(typeof(UserStatSheetSchema))]
[JsonSerializable(typeof(AislingTrackersSchema))]
[JsonSerializable(typeof(TrackersSchema))]
[JsonSerializable(typeof(MundaneIllustrationMetaSchema))]
[JsonSerializable(typeof(ChannelSettingsSchema))]
[JsonSerializable(typeof(ItemDetailsSchema))]
[JsonSerializable(typeof(BulletinBoardSchema))]
[JsonSerializable(typeof(MailBoxSchema))]
[JsonSerializable(typeof(PostSchema))]

//collections
[JsonSerializable(typeof(List<ItemSchema>))]
[JsonSerializable(typeof(List<SkillSchema>))]
[JsonSerializable(typeof(List<SpellSchema>))]
[JsonSerializable(typeof(List<EffectSchema>))]
[JsonSerializable(typeof(List<LegendMarkSchema>))]
[JsonSerializable(typeof(List<MonsterSpawnSchema>))]
[JsonSerializable(typeof(List<MerchantSpawnSchema>))]
[JsonSerializable(typeof(List<ReactorTileSchema>))]
[JsonSerializable(typeof(List<EventMetaSchema>))]
[JsonSerializable(typeof(List<MundaneIllustrationMetaSchema>))]
[JsonSerializable(typeof(List<ChannelSettingsSchema>))]
[JsonSerializable(typeof(List<ItemDetailsSchema>))]
[JsonSerializable(typeof(List<GuildRankSchema>))]
[JsonSerializable(typeof(List<PostSchema>))]
[JsonSerializable(typeof(ICollection<ItemSchema>))]
[JsonSerializable(typeof(ICollection<SkillSchema>))]
[JsonSerializable(typeof(ICollection<SpellSchema>))]
[JsonSerializable(typeof(ICollection<EffectSchema>))]
[JsonSerializable(typeof(ICollection<LegendMarkSchema>))]
[JsonSerializable(typeof(ICollection<MonsterSpawnSchema>))]
[JsonSerializable(typeof(ICollection<MerchantSpawnSchema>))]
[JsonSerializable(typeof(ICollection<ReactorTileSchema>))]
[JsonSerializable(typeof(ICollection<EventMetaSchema>))]
[JsonSerializable(typeof(ICollection<MundaneIllustrationMetaSchema>))]
[JsonSerializable(typeof(ICollection<ChannelSettingsSchema>))]
[JsonSerializable(typeof(ICollection<ItemDetailsSchema>))]
[JsonSerializable(typeof(ICollection<GuildRankSchema>))]
[JsonSerializable(typeof(ICollection<PostSchema>))]
[JsonSerializable(typeof(IEnumerable<ItemSchema>))]
[JsonSerializable(typeof(IEnumerable<SkillSchema>))]
[JsonSerializable(typeof(IEnumerable<SpellSchema>))]
[JsonSerializable(typeof(IEnumerable<EffectSchema>))]
[JsonSerializable(typeof(IEnumerable<LegendMarkSchema>))]
[JsonSerializable(typeof(IEnumerable<MonsterSpawnSchema>))]
[JsonSerializable(typeof(IEnumerable<MerchantSpawnSchema>))]
[JsonSerializable(typeof(IEnumerable<ReactorTileSchema>))]
[JsonSerializable(typeof(IEnumerable<EventMetaSchema>))]
[JsonSerializable(typeof(IEnumerable<MundaneIllustrationMetaSchema>))]
[JsonSerializable(typeof(IEnumerable<ChannelSettingsSchema>))]
[JsonSerializable(typeof(IEnumerable<ItemDetailsSchema>))]
[JsonSerializable(typeof(IEnumerable<GuildRankSchema>))]
[JsonSerializable(typeof(IEnumerable<PostSchema>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
[JsonSerializable(typeof(Dictionary<string, TimedEventCollection.Event>))]
[JsonSerializable(typeof(HashSet<string>))]

//enums
[JsonSerializable(typeof(Stat))]
[JsonSerializable(typeof(BodyAnimation))]
[JsonSerializable(typeof(AoeShape))]
[JsonSerializable(typeof(TargetFilter))]
[JsonSerializable(typeof(EquipmentType))]
[JsonSerializable(typeof(LevelCircle))]
[JsonSerializable(typeof(Element))]
[JsonSerializable(typeof(Stat?))]
[JsonSerializable(typeof(BodyAnimation?))]
[JsonSerializable(typeof(AoeShape?))]
[JsonSerializable(typeof(TargetFilter?))]
[JsonSerializable(typeof(EquipmentType?))]
[JsonSerializable(typeof(LevelCircle?))]
[JsonSerializable(typeof(Element?))]

//numerics
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(sbyte))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(ushort))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(byte?))]
[JsonSerializable(typeof(sbyte?))]
[JsonSerializable(typeof(short?))]
[JsonSerializable(typeof(ushort?))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(uint?))]
[JsonSerializable(typeof(long?))]
[JsonSerializable(typeof(ulong?))]
[JsonSerializable(typeof(decimal?))]
[JsonSerializable(typeof(float?))]
[JsonSerializable(typeof(double?))]

//other primitives
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(TimeSpan?))]

//obj
[JsonSerializable(typeof(object))]

// ReSharper disable once ClassCanBeSealed.Global No it can not
public partial class SerializationContext : JsonSerializerContext { }