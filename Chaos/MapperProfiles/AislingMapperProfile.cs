using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Factories.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Schemas.Aisling;
using Chaos.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.MapperProfiles;

public sealed class AislingMapperProfile : IMapperProfile<Aisling, AislingSchema>,
                                           IMapperProfile<Aisling, AttributesArgs>,
                                           IMapperProfile<Aisling, DisplayAislingArgs>,
                                           IMapperProfile<Aisling, ProfileArgs>,
                                           IMapperProfile<Aisling, SelfProfileArgs>,
                                           IMapperProfile<Aisling, UserIdArgs>,
                                           IMapperProfile<Aisling, WorldListMemberInfo>
{
    private readonly IExchangeFactory ExchangeFactory;
    private readonly ICloningService<Item> ItemCloner;
    private readonly ILogger<AislingMapperProfile> Logger;
    private readonly ILoggerFactory LoggerFactory;
    private readonly ITypeMapper Mapper;
    private readonly ISimpleCache SimpleCache;

    public AislingMapperProfile(
        ISimpleCache simpleCache,
        ITypeMapper mapper,
        IExchangeFactory exchangeFactory,
        ILoggerFactory loggerFactory,
        ILogger<AislingMapperProfile> logger,
        ICloningService<Item> itemCloner
    )
    {
        Mapper = mapper;
        Logger = logger;
        ItemCloner = itemCloner;
        ExchangeFactory = exchangeFactory;
        SimpleCache = simpleCache;
        LoggerFactory = loggerFactory;
    }

    public Aisling Map(AislingSchema obj)
    {
        var mapInstance = SimpleCache.Get<MapInstance>(obj.MapInstanceId);

        var aisling = new Aisling(
            obj.Name,
            mapInstance,
            new Point(obj.X, obj.Y),
            ExchangeFactory,
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
            GuildName = obj.GuildName,
            GuildTitle = obj.GuildTitle,
            HairColor = obj.HairColor,
            HairStyle = obj.HairStyle,
            Nation = obj.Nation,
            IgnoreList = new IgnoreList(obj.IgnoreList),
            Titles = new TitleList(obj.Titles),
            Options = Mapper.Map<UserOptions>(obj.UserOptions),
            UserStatSheet = Mapper.Map<UserStatSheet>(obj.StatSheet),
            IsAdmin = obj.IsAdmin
            //TODO: Effects
        };

        return aisling;
    }

    AislingSchema IMapperProfile<Aisling, AislingSchema>.Map(Aisling obj)
    {
        var ret = new AislingSchema
        {
            MapInstanceId = obj.MapInstance.InstanceId,
            BodyColor = obj.BodyColor,
            BodySprite = obj.BodySprite,
            Direction = obj.Direction,
            FaceSprite = obj.FaceSprite,
            GamePoints = obj.GamePoints,
            Gender = obj.Gender,
            Gold = obj.Gold,
            GuildName = obj.GuildName,
            GuildTitle = obj.GuildTitle,
            HairColor = obj.HairColor,
            HairStyle = obj.HairStyle,
            Name = obj.Name,
            Nation = obj.Nation,
            X = obj.X,
            Y = obj.Y,
            IsAdmin = obj.IsAdmin,
            StatSheet = Mapper.Map<UserStatSheetSchema>(obj.StatSheet),
            Titles = obj.Titles.ToList(),
            UserOptions = Mapper.Map<UserOptionsSchema>(obj.Options),
            IgnoreList = obj.IgnoreList.ToList(),

            Effects = Array.Empty<EffectSchema>()
        };

        Logger.LogTrace("Successfully mapped {Name} to schema", ret.Name);

        return ret;
    }

    public Aisling Map(AttributesArgs obj) => throw new NotImplementedException();

    AttributesArgs IMapperProfile<Aisling, AttributesArgs>.Map(Aisling obj) => new()
    {
        Ability = (byte)obj.UserStatSheet.Ability,
        Ac = (sbyte)Math.Clamp(
            obj.UserStatSheet.EffectiveAc,
            WorldOptions.Instance.MinimumAislingAc,
            WorldOptions.Instance.MaximumAislingAc),
        //TODO: blind
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
        MagicResistance = obj.UserStatSheet.EffectiveMagicResistance,
        MailFlags = MailFlag.None, //TODO: mail system
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
            var pantsColor = (byte)((overcoat?.Template?.PantsColor ?? armor?.Template.PantsColor) ?? 0);

            return new DisplayAislingArgs
            {
                AccessoryColor1 = acc1?.Color ?? DisplayColor.Default,
                AccessoryColor2 = acc2?.Color ?? DisplayColor.Default,
                AccessoryColor3 = acc3?.Color ?? DisplayColor.Default,
                AccessorySprite1 = acc1?.Template.ItemSprite.DisplaySprite ?? 0,
                AccessorySprite2 = acc2?.Template.ItemSprite.DisplaySprite ?? 0,
                AccessorySprite3 = acc3?.Template.ItemSprite.DisplaySprite ?? 0,
                ArmorSprite1 = armor?.Template.ItemSprite.DisplaySprite ?? 0, //TODO: figure this out again cuz i deleted it
                ArmorSprite2 = armor?.Template.ItemSprite.DisplaySprite ?? 0,
                BodyColor = obj.BodyColor,
                BodySprite = obj.BodySprite + pantsColor,
                BootsColor = boots?.Color ?? DisplayColor.Default,
                BootsSprite = (byte)(boots?.Template.ItemSprite.DisplaySprite ?? 0),
                CreatureType = obj.Type,
                Direction = obj.Direction,
                FaceSprite = (byte)obj.FaceSprite,
                EntityType = EntityType.Aisling,
                Gender = obj.Gender,
                GroupBoxText = null,
                HeadColor = overHelm?.Template.Color ?? helmet?.Template.Color ?? obj.HairColor,
                HeadSprite = overHelm?.Template.ItemSprite.DisplaySprite
                             ?? helmet?.Template.ItemSprite.DisplaySprite ?? (ushort)obj.HairStyle,
                Id = obj.Id,
                IsDead = obj.IsDead,
                IsHidden = false, //TODO: invisibility
                IsMaster = obj.UserStatSheet.Master,
                LanternSize = LanternSize.None, //TODO: if we add lanterns and dark maps later,
                Name = obj.Name,
                NameTagStyle = NameTagStyle.NeutralHover, //TODO: if we add pvp later
                OvercoatColor = overcoat?.Color ?? DisplayColor.Default,
                OvercoatSprite = overcoat?.Template.ItemSprite.DisplaySprite ?? 0,
                X = obj.X,
                Y = obj.Y,
                RestPosition = RestPosition.None, //TODO: if we add rest positions in later,
                ShieldSprite = (byte)(shield?.Template.ItemSprite.DisplaySprite ?? 0),
                Sprite = obj.Sprite,
                WeaponSprite = weapon?.Template.ItemSprite.DisplaySprite ?? 0
            };
        }
    }

    public Aisling Map(ProfileArgs obj) => throw new NotImplementedException();
    public Aisling Map(SelfProfileArgs obj) => throw new NotImplementedException();
    public Aisling Map(UserIdArgs obj) => throw new NotImplementedException();
    public Aisling Map(WorldListMemberInfo obj) => throw new NotImplementedException();

    WorldListMemberInfo IMapperProfile<Aisling, WorldListMemberInfo>.Map(Aisling obj) => new()
    {
        BaseClass = obj.UserStatSheet.BaseClass,
        Color = WorldListColor.White,
        IsMaster = obj.UserStatSheet.Master,
        Name = obj.Name,
        SocialStatus = obj.SocialStatus,
        Title = obj.Titles.FirstOrDefault()
    };

    UserIdArgs IMapperProfile<Aisling, UserIdArgs>.Map(Aisling obj) => new()
    {
        BaseClass = obj.UserStatSheet.BaseClass,
        Direction = obj.Direction,
        Gender = obj.Gender,
        Id = obj.Id
    };

    SelfProfileArgs IMapperProfile<Aisling, SelfProfileArgs>.Map(Aisling obj) => new()
    {
        AdvClass = obj.UserStatSheet.AdvClass,
        BaseClass = obj.UserStatSheet.BaseClass,
        Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>),
        GroupOpen = obj.Options.Group,
        GroupString = obj.Group?.ToString(),
        GuildName = obj.GuildName,
        GuildTitle = obj.GuildTitle,
        IsMaster = obj.UserStatSheet.Master,
        LegendMarks = Mapper.MapMany<LegendMarkInfo>(obj.Legend).ToList(),
        Name = obj.Name,
        Nation = obj.Nation,
        Portrait = obj.Portrait,
        ProfileText = obj.ProfileText,
        SocialStatus = obj.SocialStatus,
        SpouseName = null, //TODO: when we implement marraige i guess
        Titles = obj.Titles.ToList()
    };

    ProfileArgs IMapperProfile<Aisling, ProfileArgs>.Map(Aisling obj) => new()
    {
        AdvClass = obj.UserStatSheet.AdvClass,
        BaseClass = obj.UserStatSheet.BaseClass,
        Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>)!,
        GroupOpen = obj.Options.Group,
        GuildName = obj.GuildName,
        GuildTitle = obj.GuildTitle,
        Id = obj.Id,
        LegendMarks = Mapper.MapMany<LegendMarkInfo>(obj.Legend).ToList(),
        Name = obj.Name,
        Nation = obj.Nation,
        Portrait = obj.Portrait,
        ProfileText = obj.ProfileText,
        SocialStatus = obj.SocialStatus,
        Titles = obj.Titles.ToList()
    };
}