using System.Linq;
using AutoMapper;
using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.DataObjects.Serializable;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.WorldObjects;

namespace Chaos.Mappers;

public class UserMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ICacheManager<string, MapInstance> MapInstanceManager;
    private readonly IUserFactory UserFactory;

    public UserMapper(ICacheManager<string, MapInstance> mapInstanceManager, IUserFactory userFactory)
    {
        MapInstanceManager = mapInstanceManager;
        UserFactory = userFactory;

        CreateMap<SerializableUser, User>(MemberList.None)
            .DisableCtorValidation()
            .ForMember(u => u.MapInstance,
                o => o.MapFrom(s => MapInstanceManager.GetObject(s.MapInstanceId)))
            .ForMember(u => u.StatSheet,
                o => o.MapFrom((s, u) =>
                {
                    u.StatSheet.AttackSpeed = s.AttackSpeed;
                    u.StatSheet.CooldownReduction = s.CooldownReduction;
                    u.StatSheet.Str = s.Str;
                    u.StatSheet.Int = s.Int;
                    u.StatSheet.Wis = s.Wis;
                    u.StatSheet.Con = s.Con;
                    u.StatSheet.Dex = s.Dex;
                    u.StatSheet.UnspentPoints = s.UnspentPoints;
                    u.StatSheet.CurrentHp = s.CurrentHp;
                    u.StatSheet.CurrentMp = s.CurrentMp;
                    u.StatSheet.MaximumHp = s.MaximumHp;
                    u.StatSheet.MaximumMp = s.MaximumMp;
                    u.StatSheet.Ac = s.Ac;
                    u.StatSheet.MagicResistance = s.MagicResistance;
                    u.StatSheet.Hit = s.Hit;
                    u.StatSheet.Dmg = s.Dmg;
                    u.StatSheet.Level = s.Level;
                    u.StatSheet.ToNextLevel = s.ToNextLevel;
                    u.StatSheet.TotalExp = s.TotalExp;
                    u.StatSheet.Ability = s.Ability;
                    u.StatSheet.ToNextAbility = s.ToNextAbility;
                    u.StatSheet.TotalAbility = s.TotalAbility;
                    u.StatSheet.BaseClass = s.BaseClass;
                    u.StatSheet.AdvClass = s.AdvClass;
                    u.StatSheet.Master = s.Master;
                    u.StatSheet.MaxWeight = s.MaxWeight;
                    u.StatSheet.Nation = s.Nation;

                    return u.StatSheet;
                }));

        CreateMap<User, SerializableUser>(MemberList.None)
            .ForMember(s => s.MapInstanceId,
                o => o.MapFrom(u => u.MapInstance.InstanceId))
            .ForMember(s => s.AttackSpeed,
                o => o.MapFrom(u => u.StatSheet.AttackSpeed))
            .ForMember(s => s.CooldownReduction,
                o => o.MapFrom(u => u.StatSheet.CooldownReduction))
            .ForMember(s => s.Str,
                o => o.MapFrom(u => u.StatSheet.Str))
            .ForMember(s => s.Int,
                o => o.MapFrom(u => u.StatSheet.Int))
            .ForMember(s => s.Wis,
                o => o.MapFrom(u => u.StatSheet.Wis))
            .ForMember(s => s.Con,
                o => o.MapFrom(u => u.StatSheet.Con))
            .ForMember(s => s.Dex,
                o => o.MapFrom(u => u.StatSheet.Dex))
            .ForMember(s => s.UnspentPoints,
                o => o.MapFrom(u => u.StatSheet.UnspentPoints))
            .ForMember(s => s.CurrentHp,
                o => o.MapFrom(u => u.StatSheet.CurrentHp))
            .ForMember(s => s.CurrentMp,
                o => o.MapFrom(u => u.StatSheet.CurrentMp))
            .ForMember(s => s.MaximumHp,
                o => o.MapFrom(u => u.StatSheet.MaximumHp))
            .ForMember(s => s.MaximumMp,
                o => o.MapFrom(u => u.StatSheet.MaximumMp))
            .ForMember(s => s.Ac,
                o => o.MapFrom(u => u.StatSheet.Ac))
            .ForMember(s => s.MagicResistance,
                o => o.MapFrom(u => u.StatSheet.MagicResistance))
            .ForMember(s => s.Hit,
                o => o.MapFrom(u => u.StatSheet.Hit))
            .ForMember(s => s.Dmg,
                o => o.MapFrom(u => u.StatSheet.Dmg))
            .ForMember(s => s.Level,
                o => o.MapFrom(u => u.StatSheet.Level))
            .ForMember(s => s.ToNextLevel,
                o => o.MapFrom(u => u.StatSheet.ToNextLevel))
            .ForMember(s => s.TotalExp,
                o => o.MapFrom(u => u.StatSheet.TotalExp))
            .ForMember(s => s.Ability,
                o => o.MapFrom(u => u.StatSheet.Ability))
            .ForMember(s => s.ToNextAbility,
                o => o.MapFrom(u => u.StatSheet.ToNextAbility))
            .ForMember(s => s.TotalAbility,
                o => o.MapFrom(u => u.StatSheet.TotalAbility))
            .ForMember(s => s.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass))
            .ForMember(s => s.AdvClass,
                o => o.MapFrom(u => u.StatSheet.AdvClass))
            .ForMember(s => s.Master,
                o => o.MapFrom(u => u.StatSheet.Master))
            .ForMember(s => s.MaxWeight,
                o => o.MapFrom(u => u.StatSheet.MaxWeight))
            .ForMember(s => s.Nation,
                o => o.MapFrom(u => u.StatSheet.Nation));

        CreateMap<User, AttributesArgs>(MemberList.None)
            .ForMember(s => s.Str,
                o => o.MapFrom(u => u.StatSheet.Str))
            .ForMember(s => s.Int,
                o => o.MapFrom(u => u.StatSheet.Int))
            .ForMember(s => s.Wis,
                o => o.MapFrom(u => u.StatSheet.Wis))
            .ForMember(s => s.Con,
                o => o.MapFrom(u => u.StatSheet.Con))
            .ForMember(s => s.Dex,
                o => o.MapFrom(u => u.StatSheet.Dex))
            .ForMember(s => s.UnspentPoints,
                o => o.MapFrom(u => u.StatSheet.UnspentPoints))
            .ForMember(s => s.CurrentHp,
                o => o.MapFrom(u => u.StatSheet.CurrentHp))
            .ForMember(s => s.CurrentMp,
                o => o.MapFrom(u => u.StatSheet.CurrentMp))
            .ForMember(s => s.MaximumHp,
                o => o.MapFrom(u => u.StatSheet.MaximumHp))
            .ForMember(s => s.MaximumMp,
                o => o.MapFrom(u => u.StatSheet.MaximumMp))
            .ForMember(s => s.Ac,
                o => o.MapFrom(u => u.StatSheet.Ac))
            .ForMember(s => s.MagicResistance,
                o => o.MapFrom(u => u.StatSheet.MagicResistance))
            .ForMember(s => s.Hit,
                o => o.MapFrom(u => u.StatSheet.Hit))
            .ForMember(s => s.Dmg,
                o => o.MapFrom(u => u.StatSheet.Dmg))
            .ForMember(s => s.Level,
                o => o.MapFrom(u => u.StatSheet.Level))
            .ForMember(s => s.ToNextLevel,
                o => o.MapFrom(u => u.StatSheet.ToNextLevel))
            .ForMember(s => s.TotalExp,
                o => o.MapFrom(u => u.StatSheet.TotalExp))
            .ForMember(s => s.Ability,
                o => o.MapFrom(u => u.StatSheet.Ability))
            .ForMember(s => s.ToNextAbility,
                o => o.MapFrom(u => u.StatSheet.ToNextAbility))
            .ForMember(s => s.TotalAbility,
                o => o.MapFrom(u => u.StatSheet.TotalAbility))
            .ForMember(s => s.MaxWeight,
                o => o.MapFrom(u => u.StatSheet.MaxWeight))
            .ForMember(s => s.DefenseElement,
                o => o.MapFrom(u => u.StatSheet.DefenseElement))
            .ForMember(s => s.OffenseElement,
                o => o.MapFrom(u => u.StatSheet.OffenseElement));

        CreateMap<User, ProfileArgs>()
            .ForMember(a => a.AdvClass,
                o => o.MapFrom(u => u.StatSheet.AdvClass))
            .ForMember(a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass))
            .ForMember(a => a.Equipment,
                o => o.MapFrom(u => u.Equipment))
            .ForMember(a => a.GroupOpen,
                o => o.MapFrom(u => u.Options.Group));

        CreateMap<User, SelfProfileArgs>()
            .ForMember(a => a.AdvClass,
                o => o.MapFrom(u => u.StatSheet.AdvClass))
            .ForMember(a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass))
            .ForMember(a => a.Equipment,
                o => o.MapFrom(u => u.Equipment))
            .ForMember(a => a.GroupOpen,
                o => o.MapFrom(u => u.Options.Group))
            .ForMember(a => a.GroupString,
                o =>
                {
                    o.Condition(u => u.Group != null);
                    o.MapFrom(u => u.Group!.ToString());
                })
            .ForMember(a => a.IsMaster,
                o => o.MapFrom(u => u.StatSheet.Master));

        CreateMap<User, WorldListArg>(MemberList.None)
            .ForMember(a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass))
            .ForMember(a => a.Color,
                o => o.MapFrom(u => WorldListColor.White))
            .ForMember(a => a.IsMaster,
                o => o.MapFrom(u => u.StatSheet.Master))
            .ForMember(a => a.Title,
                o => o.MapFrom(u => u.Titles.FirstOrDefault()));

        CreateMap<User, UserIdArgs>(MemberList.None)
            .ForMember(a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass));

        CreateMap<User, DisplayAislingArgs>(MemberList.None)
            .ConstructUsing((src, rc) =>
            {
                var overHelm = src.Equipment[EquipmentSlot.OverHelm];
                var helmet = src.Equipment[EquipmentSlot.Helmet];
                var acc1 = src.Equipment[EquipmentSlot.Accessory1];
                var acc2 = src.Equipment[EquipmentSlot.Accessory2];
                var acc3 = src.Equipment[EquipmentSlot.Accessory3];
                var overCoat = src.Equipment[EquipmentSlot.Overcoat];
                var armor = src.Equipment[EquipmentSlot.Armor];
                var weapon = src.Equipment[EquipmentSlot.Weapon];
                var shield = src.Equipment[EquipmentSlot.Shield];
                var boots = src.Equipment[EquipmentSlot.Boots];

                var args = new DisplayAislingArgs
                {
                    HeadSprite = overHelm?.ItemSprite.DisplaySprite
                                 ?? helmet?.ItemSprite.DisplaySprite
                                 ?? (ushort)src.HairStyle,
                    HeadColor = overHelm?.Color
                                ?? helmet?.Color
                                ?? src.HairColor,
                    ArmorSprite1 = armor?.ItemSprite.DisplaySprite ?? 0,
                    ArmorSprite2 = armor?.ItemSprite.DisplaySprite ?? 0,
                    OvercoatSprite = overCoat?.ItemSprite.DisplaySprite ?? 0,
                    OvercoatColor = overCoat?.Color ?? 0,
                    BootsSprite = (byte)(boots?.ItemSprite.DisplaySprite ?? 0),
                    BootsColor = boots?.Color ?? DisplayColor.None,
                    ShieldSprite = (byte)(shield?.ItemSprite.DisplaySprite ?? 0),
                    WeaponSprite = weapon?.ItemSprite.DisplaySprite ?? 0,
                    AccessoryColor1 = acc1?.Color ?? DisplayColor.None,
                    AccessoryColor2 = acc2?.Color ?? DisplayColor.None,
                    AccessoryColor3 = acc3?.Color ?? DisplayColor.None,
                    AccessorySprite1 = acc1?.ItemSprite.DisplaySprite ?? 0,
                    AccessorySprite2 = acc2?.ItemSprite.DisplaySprite ?? 0,
                    AccessorySprite3 = acc3?.ItemSprite.DisplaySprite ?? 0,
                    BodySprite = src.BodySprite,
                    BodyColor = src.BodyColor,
                    Direction = src.Direction,
                    CreatureType = CreatureType.User,
                    FaceSprite = (byte)src.FaceSprite,
                    GameObjectType = GameObjectType.Misc,
                    Gender = src.Gender,
                    GroupBoxText = null,
                    Id = src.Id,
                    IsDead = !src.IsAlive,
                    IsHidden = false,
                    IsMaster = src.StatSheet.Master,
                    LanternSize = LanternSize.None,
                    Name = src.Name,
                    NameTagStyle = NameTagStyle.NeutralHover,
                    Point = src.Point,
                    RestPosition = RestPosition.None,
                    Sprite = src.Sprite == 0 ? null : src.Sprite
                };

                return args;
            });

        //TODO: blind
        //TODO: mailflags
    }
}