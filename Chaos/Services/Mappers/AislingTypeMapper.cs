using Chaos.Common.Definitions;
using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Services.Utility.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Mappers;

public class AislingTypeMapper : ITypeMapper<Aisling, AislingSchema>,
                                 ITypeMapper<Aisling, AttributesArgs>,
                                 ITypeMapper<Aisling, DisplayAislingArgs>,
                                 ITypeMapper<Aisling, ProfileArgs>,
                                 ITypeMapper<Aisling, SelfProfileArgs>,
                                 ITypeMapper<Aisling, UserIdArgs>,
                                 ITypeMapper<Aisling, WorldListMemberInfo>
{
    private readonly IExchangeFactory ExchangeFactory;
    private readonly ICloningService<Item> ItemCloner;
    private readonly ILogger<AislingTypeMapper> Logger;
    private readonly ILoggerFactory LoggerFactory;
    private readonly ITypeMapper Mapper;
    private readonly ISimpleCache SimpleCache;

    public AislingTypeMapper(
        ISimpleCache simpleCache,
        ITypeMapper mapper,
        ICloningService<Item> itemCloner,
        ILoggerFactory loggerFactory,
        ILogger<AislingTypeMapper> logger,
        IExchangeFactory exchangeFactory
    )
    {
        Mapper = mapper;
        Logger = logger;
        ExchangeFactory = exchangeFactory;
        SimpleCache = simpleCache;
        ItemCloner = itemCloner;
        LoggerFactory = loggerFactory;
    }

    public Aisling Map(AislingSchema obj) => new(
        obj,
        SimpleCache,
        ItemCloner,
        Mapper,
        ExchangeFactory,
        LoggerFactory.CreateLogger<Aisling>());

    public AislingSchema Map(Aisling obj)
    {
        var ret = new AislingSchema
        {
            MapInstanceId = obj.MapInstance.InstanceId,
            BankedGold = obj.Bank.Gold,
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

            StatSheet = Mapper.Map<UserStatSheetSchema>(obj.StatSheet),
            Titles = obj.Titles.ToList(),
            UserOptions = Mapper.Map<UserOptionsSchema>(obj.Options),
            IgnoreList = obj.IgnoreList.ToList(),

            Legend = obj.Legend
                        .Select(Mapper.Map<LegendMarkSchema>)
                        .ToList(),

            Bank = obj.Bank
                      .Select(Mapper.Map<ItemSchema>)
                      .ToList(),

            Equipment = obj.Equipment
                           .Select(Mapper.Map<ItemSchema>)
                           .ToList(),

            Inventory = obj.Inventory
                           .Select(Mapper.Map<ItemSchema>)
                           .ToList(),

            SkillBook = obj.SkillBook
                           .Select(Mapper.Map<SkillSchema>)
                           .ToList(),

            SpellBook = obj.SpellBook
                           .Select(Mapper.Map<SpellSchema>)
                           .ToList(),

            Effects = Array.Empty<EffectSchema>()
        };

        Logger.LogTrace("Successfully mapped {Name} to schema", ret.Name);

        return ret;
    }

    public Aisling Map(AttributesArgs obj) => throw new NotImplementedException();

    AttributesArgs ITypeMapper<Aisling, AttributesArgs>.Map(Aisling obj) => new()
    {
        Ability = (byte)obj.StatSheet.Ability,
        Ac = obj.StatSheet.EffectiveAc,
        //TODO: blind
        Con = obj.StatSheet.EffectiveCon,
        CurrentHp = (uint)Math.Clamp(obj.StatSheet.CurrentHp, 0, int.MaxValue),
        CurrentMp = (uint)Math.Clamp(obj.StatSheet.CurrentMp, 0, int.MaxValue),
        CurrentWeight = (short)obj.StatSheet.CurrentWeight,
        DefenseElement = obj.StatSheet.DefenseElement,
        Dex = obj.StatSheet.EffectiveDex,
        Dmg = obj.StatSheet.EffectiveDmg,
        GamePoints = (uint)obj.GamePoints,
        Gold = (uint)obj.Gold,
        Hit = obj.StatSheet.EffectiveHit,
        Int = obj.StatSheet.EffectiveInt,
        IsAdmin = obj.IsAdmin,
        Level = (byte)obj.StatSheet.Level,
        MagicResistance = obj.StatSheet.EffectiveMagicResistance,
        MailFlags = MailFlag.None, //TODO: mail system
        MaximumHp = obj.StatSheet.EffectiveMaximumHp,
        MaximumMp = obj.StatSheet.EffectiveMaximumMp,
        MaxWeight = (short)obj.StatSheet.MaxWeight,
        OffenseElement = obj.StatSheet.OffenseElement,
        Str = obj.StatSheet.EffectiveStr,
        ToNextAbility = obj.StatSheet.ToNextAbility,
        ToNextLevel = obj.StatSheet.ToNextLevel,
        TotalAbility = obj.StatSheet.TotalAbility,
        TotalExp = obj.StatSheet.TotalExp,
        UnspentPoints = (byte)obj.StatSheet.UnspentPoints,
        Wis = obj.StatSheet.EffectiveWis
    };

    public Aisling Map(DisplayAislingArgs obj) => throw new NotImplementedException();

    DisplayAislingArgs ITypeMapper<Aisling, DisplayAislingArgs>.Map(Aisling obj)
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

            return new DisplayAislingArgs
            {
                AccessoryColor1 = acc1?.Color ?? DisplayColor.None,
                AccessoryColor2 = acc2?.Color ?? DisplayColor.None,
                AccessoryColor3 = acc3?.Color ?? DisplayColor.None,
                AccessorySprite1 = acc1?.Template.PanelSprite ?? 0,
                AccessorySprite2 = acc2?.Template.PanelSprite ?? 0,
                AccessorySprite3 = acc3?.Template.PanelSprite ?? 0,
                ArmorSprite1 = armor?.Template.PanelSprite ?? 0, //TODO: figure this out again cuz i deleted it
                ArmorSprite2 = armor?.Template.PanelSprite ?? 0,
                BodyColor = obj.BodyColor,
                BodySprite = obj.BodySprite,
                BootsColor = boots?.Color ?? DisplayColor.None,
                BootsSprite = (byte)(boots?.Template.PanelSprite ?? 0),
                CreatureType = obj.Type,
                Direction = obj.Direction,
                FaceSprite = (byte)obj.FaceSprite,
                GameObjectType = GameObjectType.Misc,
                Gender = obj.Gender,
                GroupBoxText = null,
                HeadColor = overHelm?.Template.Color ?? helmet?.Template.Color ?? obj.HairColor,
                HeadSprite = overHelm?.Template.PanelSprite ?? helmet?.Template.PanelSprite ?? (ushort)obj.HairStyle,
                Id = obj.Id,
                IsDead = !obj.IsAlive,
                IsHidden = false, //TODO: invisibility
                IsMaster = obj.StatSheet.Master,
                LanternSize = LanternSize.None, //TODO: if we add lanterns and dark maps later,
                Name = obj.Name,
                NameTagStyle = NameTagStyle.NeutralHover, //TODO: if we add pvp later
                OvercoatColor = overcoat?.Color ?? DisplayColor.None,
                OvercoatSprite = overcoat?.Template.PanelSprite ?? 0,
                X = obj.X,
                Y = obj.Y,
                RestPosition = RestPosition.None, //TODO: if we add rest positions in later,
                ShieldSprite = (byte)(shield?.Template.PanelSprite ?? 0),
                Sprite = obj.Sprite,
                WeaponSprite = weapon?.Template.PanelSprite ?? 0
            };
        }
    }

    public Aisling Map(ProfileArgs obj) => throw new NotImplementedException();
    public Aisling Map(SelfProfileArgs obj) => throw new NotImplementedException();
    public Aisling Map(UserIdArgs obj) => throw new NotImplementedException();
    public Aisling Map(WorldListMemberInfo obj) => throw new NotImplementedException();

    WorldListMemberInfo ITypeMapper<Aisling, WorldListMemberInfo>.Map(Aisling obj) => new()
    {
        BaseClass = obj.StatSheet.BaseClass,
        Color = WorldListColor.White,
        IsMaster = obj.StatSheet.Master,
        Name = obj.Name,
        SocialStatus = obj.SocialStatus,
        Title = obj.Titles.FirstOrDefault()
    };

    UserIdArgs ITypeMapper<Aisling, UserIdArgs>.Map(Aisling obj) => new()
    {
        BaseClass = obj.StatSheet.BaseClass,
        Direction = obj.Direction,
        Gender = obj.Gender,
        Id = obj.Id
    };

    SelfProfileArgs ITypeMapper<Aisling, SelfProfileArgs>.Map(Aisling obj) => new()
    {
        AdvClass = obj.StatSheet.AdvClass,
        BaseClass = obj.StatSheet.BaseClass,
        Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>),
        GroupOpen = obj.Options.Group,
        GroupString = null, //TODO: when we implement group box
        GuildName = obj.GuildName,
        GuildTitle = obj.GuildTitle,
        IsMaster = obj.StatSheet.Master,
        LegendMarks = obj.Legend.Select(l => l.ToLegendMarkInfo()).ToList(),
        Name = obj.Name,
        Nation = obj.Nation,
        Portrait = obj.Portrait,
        ProfileText = obj.ProfileText,
        SocialStatus = obj.SocialStatus,
        SpouseName = null, //TODO: when we implement marraige i guess
        Titles = obj.Titles.ToList()
    };

    ProfileArgs ITypeMapper<Aisling, ProfileArgs>.Map(Aisling obj) => new()
    {
        AdvClass = obj.StatSheet.AdvClass,
        BaseClass = obj.StatSheet.BaseClass,
        Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>)!,
        GroupOpen = obj.Options.Group,
        GuildName = obj.GuildName,
        GuildTitle = obj.GuildTitle,
        Id = obj.Id,
        LegendMarks = obj.Legend.Select(l => l.ToLegendMarkInfo()).ToList(),
        Name = obj.Name,
        Nation = obj.Nation,
        Portrait = obj.Portrait,
        ProfileText = obj.ProfileText,
        SocialStatus = obj.SocialStatus,
        Titles = obj.Titles.ToList()
    };
}