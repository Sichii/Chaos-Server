using System.Linq;
using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.Objects.Serializable;
using Chaos.Objects.World;

namespace Chaos.Mappers;

public class UserMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ISimpleCache<string, MapInstance> MapInstanceCache;

    public UserMapper(ISimpleCache<string, MapInstance> mapInstanceCache)
    {
        MapInstanceCache = mapInstanceCache;

        CreateMap<SerializableUser, User>(MemberList.None)
            .DisableCtorValidation()
            .BeforeMap(
                (src, dest) =>
                {
                    dest.MapInstance = MapInstanceCache.GetObject(src.MapInstanceId);
                    ShallowCopy<UserStatSheet>.Merge(src.StatSheet, dest.StatSheet);
                });

        CreateMap<User, SerializableUser>(MemberList.None)
            .ForMember(
                s => s.MapInstanceId,
                o => o.MapFrom(u => u.MapInstance.InstanceId))
            .ForMember(
                s => s.Nation,
                o => o.MapFrom(u => u.StatSheet.Nation));

        CreateMap<User, AttributesArgs>(MemberList.Destination)
            .ForMember(
                s => s.Ability,
                o => o.MapFrom(u => u.StatSheet.Ability))
            .ForMember(
                s => s.Ac,
                o => o.MapFrom(u => u.StatSheet.EffectiveAc))
            .ForMember(
                s => s.Blind,
                o => o.MapFrom(u => false))
            .ForMember(
                s => s.Con,
                o => o.MapFrom(u => u.StatSheet.EffectiveCon))
            .ForMember(
                s => s.CurrentHp,
                o => o.MapFrom(u => u.StatSheet.CurrentHp))
            .ForMember(
                s => s.CurrentMp,
                o => o.MapFrom(u => u.StatSheet.CurrentMp))
            .ForMember(
                s => s.CurrentWeight,
                o => o.MapFrom(u => u.StatSheet.CurrentWeight))
            .ForMember(
                s => s.DefenseElement,
                o => o.MapFrom(u => u.StatSheet.DefenseElement))
            .ForMember(
                s => s.Dex,
                o => o.MapFrom(u => u.StatSheet.EffectiveDex))
            .ForMember(
                s => s.Dmg,
                o => o.MapFrom(u => u.StatSheet.EffectiveDmg))
            .ForMember(
                s => s.GamePoints,
                o => o.MapFrom(u => u.GamePoints))
            .ForMember(
                s => s.Gold,
                o => o.MapFrom(u => u.Gold))
            .ForMember(
                s => s.Hit,
                o => o.MapFrom(u => u.StatSheet.EffectiveHit))
            .ForMember(
                s => s.Int,
                o => o.MapFrom(u => u.StatSheet.EffectiveInt))
            .ForMember(
                s => s.IsAdmin,
                o => o.MapFrom(u => u.IsAdmin))
            .ForMember(
                s => s.Level,
                o => o.MapFrom(u => u.StatSheet.Level))
            .ForMember(
                s => s.MagicResistance,
                o => o.MapFrom(u => u.StatSheet.EffectiveMagicResistance))
            .ForMember(
                s => s.MailFlags,
                o => o.MapFrom(u => MailFlag.None))
            .ForMember(
                s => s.MaximumHp,
                o => o.MapFrom(u => u.StatSheet.EffectiveMaximumHp))
            .ForMember(
                s => s.MaximumMp,
                o => o.MapFrom(u => u.StatSheet.EffectiveMaximumMp))
            .ForMember(
                s => s.MaxWeight,
                o => o.MapFrom(u => u.StatSheet.MaxWeight))
            .ForMember(
                s => s.OffenseElement,
                o => o.MapFrom(u => u.StatSheet.OffenseElement))
            .ForMember(
                s => s.StatUpdateType,
                o => o.Ignore())
            .ForMember(
                s => s.Str,
                o => o.MapFrom(u => u.StatSheet.EffectiveStr))
            .ForMember(
                s => s.ToNextAbility,
                o => o.MapFrom(u => u.StatSheet.ToNextAbility))
            .ForMember(
                s => s.ToNextLevel,
                o => o.MapFrom(u => u.StatSheet.ToNextLevel))
            .ForMember(
                s => s.TotalAbility,
                o => o.MapFrom(u => u.StatSheet.TotalAbility))
            .ForMember(
                s => s.TotalExp,
                o => o.MapFrom(u => u.StatSheet.TotalExp))
            .ForMember(
                s => s.UnspentPoints,
                o => o.MapFrom(u => u.StatSheet.UnspentPoints))
            .ForMember(
                s => s.Wis,
                o => o.MapFrom(u => u.StatSheet.EffectiveWis));

        CreateMap<User, ProfileArgs>()
            .ForMember(
                a => a.AdvClass,
                o => o.MapFrom(u => u.StatSheet.AdvClass))
            .ForMember(
                a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass))
            .ForMember(
                a => a.Equipment,
                o => o.MapFrom(u => u.Equipment))
            .ForMember(
                a => a.GroupOpen,
                o => o.MapFrom(u => u.Options.Group));

        CreateMap<User, SelfProfileArgs>(MemberList.None)
            .ForMember(
                a => a.AdvClass,
                o => o.MapFrom(u => u.StatSheet.AdvClass))
            .ForMember(
                a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass))
            .ForMember(
                a => a.Equipment,
                o => o.MapFrom(u => u.Equipment))
            .ForMember(
                a => a.GroupOpen,
                o => o.MapFrom(u => u.Options.Group))
            .ForMember(
                a => a.GroupString,
                o => o.MapFrom(u => u.Group != null ? u.Group.ToString() : null))
            .ForMember(
                a => a.IsMaster,
                o => o.MapFrom(u => u.StatSheet.Master));

        CreateMap<User, WorldListArg>(MemberList.None)
            .ForMember(
                a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass))
            .ForMember(
                a => a.Color,
                o => o.MapFrom(u => WorldListColor.White))
            .ForMember(
                a => a.IsMaster,
                o => o.MapFrom(u => u.StatSheet.Master))
            .ForMember(
                a => a.Title,
                o => o.MapFrom(u => u.Titles.FirstOrDefault()));

        CreateMap<User, UserIdArgs>(MemberList.None)
            .ForMember(
                a => a.BaseClass,
                o => o.MapFrom(u => u.StatSheet.BaseClass));

        CreateMap<User, DisplayAislingArgs>(MemberList.None)
            .ConstructUsing(
                (src, rc) =>
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
                        HeadSprite = overHelm?.Template.ItemSprite.DisplaySprite
                                     ?? helmet?.Template.ItemSprite.DisplaySprite
                                     ?? (ushort)src.HairStyle,
                        HeadColor = overHelm?.Color
                                    ?? helmet?.Color
                                    ?? src.HairColor,
                        ArmorSprite1 = armor?.Template.ItemSprite.DisplaySprite ?? 0,
                        ArmorSprite2 = armor?.Template.ItemSprite.DisplaySprite ?? 0,
                        OvercoatSprite = overCoat?.Template.ItemSprite.DisplaySprite ?? 0,
                        OvercoatColor = overCoat?.Color ?? 0,
                        BootsSprite = (byte)(boots?.Template.ItemSprite.DisplaySprite ?? 0),
                        BootsColor = boots?.Color ?? DisplayColor.None,
                        ShieldSprite = (byte)(shield?.Template.ItemSprite.DisplaySprite ?? 0),
                        WeaponSprite = weapon?.Template.ItemSprite.DisplaySprite ?? 0,
                        AccessoryColor1 = acc1?.Color ?? DisplayColor.None,
                        AccessoryColor2 = acc2?.Color ?? DisplayColor.None,
                        AccessoryColor3 = acc3?.Color ?? DisplayColor.None,
                        AccessorySprite1 = acc1?.Template.ItemSprite.DisplaySprite ?? 0,
                        AccessorySprite2 = acc2?.Template.ItemSprite.DisplaySprite ?? 0,
                        AccessorySprite3 = acc3?.Template.ItemSprite.DisplaySprite ?? 0,
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