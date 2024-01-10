using Chaos.Collections;
using Chaos.Collections.Synchronized;
using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.MapperProfiles;

public sealed class AislingMapperProfile(
    ISimpleCache simpleCache,
    ITypeMapper mapper,
    IFactory<Exchange> exchangeFactory,
    ILoggerFactory loggerFactory,
    ICloningService<Item> itemCloner,
    IScriptProvider scriptProvider,
    IStore<Guild> guildStore) : IMapperProfile<Aisling, AislingSchema>,
                                IMapperProfile<Aisling, AttributesArgs>,
                                IMapperProfile<Aisling, DisplayAislingArgs>,
                                IMapperProfile<Aisling, OtherProfileArgs>,
                                IMapperProfile<Aisling, SelfProfileArgs>,
                                IMapperProfile<Aisling, UserIdArgs>,
                                IMapperProfile<Aisling, WorldListMemberInfo>
{
    private readonly IFactory<Exchange> ExchangeFactory = exchangeFactory;
    private readonly IStore<Guild> GuildStore = guildStore;
    private readonly ICloningService<Item> ItemCloner = itemCloner;
    private readonly ILoggerFactory LoggerFactory = loggerFactory;
    private readonly ITypeMapper Mapper = mapper;
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    public Aisling Map(AislingSchema obj)
    {
        MapInstance mapInstance;
        IPoint point = new Point(obj.X, obj.Y);

        try
        {
            mapInstance = SimpleCache.Get<MapInstance>(obj.MapInstanceId);
        } catch (Exception e)
        {
            if (obj.FallbackLocation is not null)
                try
                {
                    mapInstance = SimpleCache.Get<MapInstance>(obj.FallbackLocation.Map);
                    point = obj.FallbackLocation;
                } catch (Exception ex)
                {
                    throw new KeyNotFoundException(
                        $"Unable to find {nameof(obj.MapInstanceId)} of \"{obj.MapInstanceId}\" or {nameof(obj.FallbackLocation)
                        } of \"{obj.FallbackLocation.Map}\"",
                        ex);
                }
            else
                throw new KeyNotFoundException(
                    $"Unable to find {nameof(obj.MapInstanceId)} of \"{obj.MapInstanceId}\", and no {nameof(obj.FallbackLocation)
                    } was specified",
                    e);
        }

        var aisling = new Aisling(
            obj.Name,
            mapInstance,
            point,
            ExchangeFactory,
            ScriptProvider,
            LoggerFactory.CreateLogger<Aisling>(),
            ItemCloner)
        {
            BodyColor = obj.BodyColor,
            BodySprite = obj.BodySprite,
            Direction = obj.Direction,
            FaceSprite = obj.FaceSprite,
            GamePoints = obj.GamePoints,
            Gender = obj.Gender,
            Gold = obj.Gold,
            HairColor = obj.HairColor,
            HairStyle = obj.HairStyle,
            Nation = obj.Nation,
            IgnoreList = new IgnoreList(obj.IgnoreList),
            Titles = new TitleList(obj.Titles),
            Options = Mapper.Map<UserOptions>(obj.UserOptions),
            UserStatSheet = Mapper.Map<UserStatSheet>(obj.StatSheet),
            IsAdmin = obj.IsAdmin,
            IsDead = obj.IsDead,
            ChannelSettings = new SynchronizedHashSet<ChannelSettings>(Mapper.MapMany<ChannelSettings>(obj.ChannelSettings))
        };

        //lookup guild and attach
        if (!string.IsNullOrEmpty(obj.GuildName) && GuildStore.Exists(obj.GuildName))
        {
            var guild = GuildStore.Load(obj.GuildName);
            aisling.Guild = guild;
        }

        return aisling;
    }

    AislingSchema IMapperProfile<Aisling, AislingSchema>.Map(Aisling obj)
    {
        var ret = new AislingSchema
        {
            MapInstanceId = obj.MapInstance.InstanceId,
            FallbackLocation = obj.MapInstance.ShardingOptions?.ExitLocation,
            BodyColor = obj.BodyColor,
            BodySprite = obj.BodySprite,
            Direction = obj.Direction,
            FaceSprite = obj.FaceSprite,
            GamePoints = obj.GamePoints,
            Gender = obj.Gender,
            Gold = obj.Gold,
            GuildName = obj.Guild?.Name,
            HairColor = obj.HairColor,
            HairStyle = obj.HairStyle,
            Name = obj.Name,
            Nation = obj.Nation,
            X = obj.X,
            Y = obj.Y,
            IsAdmin = obj.IsAdmin,
            IsDead = obj.IsDead,
            StatSheet = Mapper.Map<UserStatSheetSchema>(obj.StatSheet),
            Titles = obj.Titles.ToList(),
            UserOptions = Mapper.Map<UserOptionsSchema>(obj.Options),
            IgnoreList = obj.IgnoreList.ToList(),
            ChannelSettings = Mapper.MapMany<ChannelSettingsSchema>(obj.ChannelSettings)
                                    .ToList()
        };

        return ret;
    }

    public Aisling Map(AttributesArgs obj) => throw new NotImplementedException();

    AttributesArgs IMapperProfile<Aisling, AttributesArgs>.Map(Aisling obj)
        => new()
        {
            Ability = (byte)obj.UserStatSheet.Ability,
            Ac = (sbyte)Math.Clamp(
                obj.UserStatSheet.EffectiveAc,
                WorldOptions.Instance.MinimumAislingAc,
                WorldOptions.Instance.MaximumAislingAc),
            Blind = obj.Script.IsBlind(),
            Con = obj.UserStatSheet.EffectiveCon,
            CurrentHp = (uint)Math.Clamp(obj.UserStatSheet.CurrentHp, 0, int.MaxValue),
            CurrentMp = (uint)Math.Clamp(obj.UserStatSheet.CurrentMp, 0, int.MaxValue),
            CurrentWeight = (short)obj.UserStatSheet.CurrentWeight,
            DefenseElement = obj.UserStatSheet.DefenseElement,
            Dex = obj.UserStatSheet.EffectiveDex,
            Dmg = obj.UserStatSheet.EffectiveDmg,
            GamePoints = (uint)obj.GamePoints,
            Gold = (uint)obj.Gold,
            Hit = obj.UserStatSheet.EffectiveHit,
            Int = obj.UserStatSheet.EffectiveInt,
            IsAdmin = obj.IsAdmin,
            Level = (byte)obj.UserStatSheet.Level,
            HasUnreadMail = obj.MailBox.Any(post => post.IsHighlighted),
            MagicResistance = (byte)(obj.UserStatSheet.EffectiveMagicResistance / 10),
            MaximumHp = obj.UserStatSheet.EffectiveMaximumHp,
            MaximumMp = obj.UserStatSheet.EffectiveMaximumMp,
            MaxWeight = (short)obj.UserStatSheet.MaxWeight,
            OffenseElement = obj.UserStatSheet.OffenseElement,
            Str = obj.UserStatSheet.EffectiveStr,
            ToNextAbility = obj.UserStatSheet.ToNextAbility,
            ToNextLevel = obj.UserStatSheet.ToNextLevel,
            TotalAbility = obj.UserStatSheet.TotalAbility,
            TotalExp = obj.UserStatSheet.TotalExp,
            UnspentPoints = (byte)obj.UserStatSheet.UnspentPoints,
            Wis = obj.UserStatSheet.EffectiveWis
        };

    public Aisling Map(DisplayAislingArgs obj) => throw new NotImplementedException();

    DisplayAislingArgs IMapperProfile<Aisling, DisplayAislingArgs>.Map(Aisling obj)
    {
        {
            var weapon = obj.Equipment[EquipmentSlot.Weapon];
            var armor = obj.Equipment[EquipmentSlot.Armor];
            var shield = obj.Equipment[EquipmentSlot.Shield];
            var overHelm = obj.Equipment[EquipmentSlot.OverHelm];
            var helmet = obj.Equipment[EquipmentSlot.Helmet];
            var boots = obj.Equipment[EquipmentSlot.Boots];
            var acc1 = obj.Equipment[EquipmentSlot.Accessory1];
            var acc2 = obj.Equipment[EquipmentSlot.Accessory2];
            var acc3 = obj.Equipment[EquipmentSlot.Accessory3];
            var overcoat = obj.Equipment[EquipmentSlot.Overcoat];
            var pantsColor = overcoat?.Template.PantsColor ?? armor?.Template.PantsColor;

            DisplayColor headColor;

            //use overhelm color if it is dyeable or if it is not default
            if ((overHelm != null) && (overHelm.Template.IsDyeable || (overHelm.Color != DisplayColor.Default)))
                headColor = overHelm.Color;

            //use helmet color if it is dyeable or if it is not default
            else if ((helmet != null) && (helmet.Template.IsDyeable || (helmet.Color != DisplayColor.Default)))
                headColor = helmet.Color;
            else
                headColor = obj.HairColor;

            return new DisplayAislingArgs
            {
                AccessoryColor1 = acc1?.Color ?? DisplayColor.Default,
                AccessoryColor2 = acc2?.Color ?? DisplayColor.Default,
                AccessoryColor3 = acc3?.Color ?? DisplayColor.Default,
                AccessorySprite1 = acc1?.ItemSprite.DisplaySprite ?? 0,
                AccessorySprite2 = acc2?.ItemSprite.DisplaySprite ?? 0,
                AccessorySprite3 = acc3?.ItemSprite.DisplaySprite ?? 0,
                ArmorSprite1 = armor?.ItemSprite.DisplaySprite ?? 0, //TODO: figure this out again cuz i deleted it
                ArmorSprite2 = armor?.ItemSprite.DisplaySprite ?? 0,
                BodyColor = obj.BodyColor,
                BodySprite = obj.BodySprite,
                PantsColor = pantsColor,
                BootsColor = boots?.Color ?? DisplayColor.Default,
                BootsSprite = (byte)(boots?.ItemSprite.DisplaySprite ?? 0),
                Direction = obj.Direction,
                FaceSprite = (byte)obj.FaceSprite,
                Gender = obj.Gender,
                GroupBoxText = null,
                HeadColor = headColor,
                HeadSprite = overHelm?.ItemSprite.DisplaySprite ?? helmet?.ItemSprite.DisplaySprite ?? (ushort)obj.HairStyle,
                Id = obj.Id,
                IsDead = obj.IsDead,
                IsHidden = false, //"Hidden" people are unobservable, so this packet wont even be sent
                IsTransparent = obj.Visibility is VisibilityType.Hidden or VisibilityType.TrueHidden or VisibilityType.GmHidden,
                LanternSize = obj.LanternSize,
                Name = obj.Name,
                NameTagStyle = NameTagStyle.NeutralHover, //this is a default value
                OvercoatColor = overcoat?.Color ?? DisplayColor.Default,
                OvercoatSprite = overcoat?.ItemSprite.DisplaySprite ?? 0,
                X = obj.X,
                Y = obj.Y,
                RestPosition = obj.RestPosition,
                ShieldSprite = (byte)(shield?.ItemSprite.DisplaySprite ?? 0),
                Sprite = obj.Sprite == 0 ? null : obj.Sprite,
                WeaponSprite = weapon?.ItemSprite.DisplaySprite ?? 0
            };
        }
    }

    public Aisling Map(OtherProfileArgs obj) => throw new NotImplementedException();

    OtherProfileArgs IMapperProfile<Aisling, OtherProfileArgs>.Map(Aisling obj)
        => new()
        {
            DisplayClass = obj.UserStatSheet.AdvClass != AdvClass.None
                ? obj.UserStatSheet.AdvClass.ToString()
                : obj.UserStatSheet.Master
                    ? "Master"
                    : obj.UserStatSheet.BaseClass.ToString(),
            BaseClass = obj.UserStatSheet.BaseClass,
            Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>)!,
            GroupOpen = obj.Options.AllowGroup,
            GuildName = obj.Guild?.Name,
            GuildRank = obj.GuildRank,
            Id = obj.Id,
            LegendMarks = Mapper.MapMany<LegendMarkInfo>(obj.Legend)
                                .ToList(),
            Name = obj.Name,
            Nation = obj.Nation,
            Portrait = obj.Portrait,
            ProfileText = obj.ProfileText,
            SocialStatus = obj.Options.SocialStatus,
            Title = obj.Titles.FirstOrDefault()
        };

    public Aisling Map(SelfProfileArgs obj) => throw new NotImplementedException();

    SelfProfileArgs IMapperProfile<Aisling, SelfProfileArgs>.Map(Aisling obj)
        => new()
        {
            DisplayClass = obj.UserStatSheet.AdvClass != AdvClass.None
                ? obj.UserStatSheet.AdvClass.ToString()
                : obj.UserStatSheet.Master
                    ? "Master"
                    : obj.UserStatSheet.BaseClass.ToString(),
            BaseClass = obj.UserStatSheet.BaseClass,
            Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>),
            GroupOpen = obj.Options.AllowGroup,
            GroupString = obj.Group?.ToString(),
            GuildName = obj.Guild?.Name,
            GuildRank = obj.GuildRank,
            EnableMasterAbilityMetaData = obj.UserStatSheet.Master,
            EnableMasterQuestMetaData = obj.UserStatSheet.Master,
            LegendMarks = Mapper.MapMany<LegendMarkInfo>(obj.Legend)
                                .ToList(),
            Name = obj.Name,
            Nation = obj.Nation,
            Portrait = obj.Portrait,
            ProfileText = obj.ProfileText,
            SpouseName = null, //TODO: when we implement marraige i guess
            Title = obj.Titles.FirstOrDefault()
        };

    public Aisling Map(UserIdArgs obj) => throw new NotImplementedException();

    UserIdArgs IMapperProfile<Aisling, UserIdArgs>.Map(Aisling obj)
        => new()
        {
            BaseClass = obj.UserStatSheet.BaseClass,
            Direction = obj.Direction,
            Id = obj.Id
        };

    public Aisling Map(WorldListMemberInfo obj) => throw new NotImplementedException();

    WorldListMemberInfo IMapperProfile<Aisling, WorldListMemberInfo>.Map(Aisling obj)
        => new()
        {
            BaseClass = obj.UserStatSheet.BaseClass,
            Color = WorldListColor.White,
            IsMaster = obj.UserStatSheet.Master,
            Name = obj.Name,
            SocialStatus = obj.Options.SocialStatus,
            Title = obj.Titles.FirstOrDefault()
        };
}