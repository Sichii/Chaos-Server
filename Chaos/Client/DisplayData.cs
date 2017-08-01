using Newtonsoft.Json;
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class DisplayData
    {
        [JsonProperty]
        internal User User { get; set; }

        //Base
        [JsonProperty]
        internal ushort HairSprite { get; set; }
        [JsonProperty]
        internal byte HairColor { get; set; }
        [JsonProperty]
        internal byte BodySprite { get; set; }
        [JsonProperty]
        internal BodyColor BodyColor { get; set; }
        [JsonProperty]
        internal byte FaceSprite { get; set; }

        //Head
        internal ushort HeadSprite => User.Equipment[(byte)EquipmentSlot.OverHelm]?.Sprite ?? User.Equipment[(byte)EquipmentSlot.Helmet]?.Sprite ?? HairSprite;
        internal byte HeadColor => User.Equipment[(byte)EquipmentSlot.OverHelm]?.Color ?? User.Equipment[(byte)EquipmentSlot.Helmet]?.Color ?? HairColor;

        //Body
        internal ushort ArmorSprite1 => User.Equipment[(byte)EquipmentSlot.Armor]?.Sprite ?? 0;
        [JsonProperty]
        internal ushort ArmorSprite2 = 0;
        internal ushort OvercoatSprite => User.Equipment[(byte)EquipmentSlot.Overcoat]?.Sprite ?? 0;
        internal byte OvercoatColor => User.Equipment[(byte)EquipmentSlot.Overcoat]?.Color ?? 0;

        //Boots
        internal byte BootsSprite => (byte)(User.Equipment[(byte)EquipmentSlot.Boots]?.Sprite ?? 0);
        internal byte BootsColor => User.Equipment[(byte)EquipmentSlot.Boots]?.Color ?? 0;

        //Hands
        internal byte ShieldSprite => (byte)(User.Equipment[(byte)EquipmentSlot.Shield]?.Sprite ?? 0);
        internal ushort WeaponSprite => (byte)(User.Equipment[(byte)EquipmentSlot.Weapon]?.Sprite ?? 0);

        //Accessories
        internal byte AccessoryColor1 => User.Equipment[(byte)EquipmentSlot.Accessory1]?.Color ?? 0;
        internal byte AccessoryColor2 => User.Equipment[(byte)EquipmentSlot.Accessory2]?.Color ?? 0;
        internal byte AccessoryColor3 => User.Equipment[(byte)EquipmentSlot.Accessory3]?.Color ?? 0;
        internal ushort AccessorySprite1 => User.Equipment[(byte)EquipmentSlot.Accessory1]?.Sprite ?? 0;
        internal ushort AccessorySprite2 => User.Equipment[(byte)EquipmentSlot.Accessory2]?.Sprite ?? 0;
        internal ushort AccessorySprite3 => User.Equipment[(byte)EquipmentSlot.Accessory3]?.Sprite ?? 0;

        //Other
        [JsonProperty]
        internal LanternSize LanternSize { get; set; }
        [JsonProperty]
        internal NameTagStyle NameTagStyle { get; set; }
        [JsonProperty]
        internal string GroupName { get; set; }
        [JsonProperty]
        internal RestPosition RestPosition { get; set; }
        [JsonProperty]
        internal bool IsHidden { get; set; }

        /// <summary>
        /// Object containing the methods and information that is used to display the <c>User</c>.
        /// </summary>
        /// <param name="user"><c>User</c> to be displayed.</param>
        /// <param name="hairSprite">Base sprite for the hair of the user.</param>
        /// <param name="hairColor">Base color value for the hair of the user.</param>
        /// <param name="bodySprite">Base body value for the user.</param>
        internal DisplayData(User user, ushort hairSprite, byte hairColor, byte bodySprite)
        {
            User = user;
            HairSprite = hairSprite;
            HairColor = hairColor;
            BodySprite = bodySprite;
            BodyColor = BodyColor.White;
            FaceSprite = 1;
            LanternSize = LanternSize.None;
            NameTagStyle = NameTagStyle.NeutralHover;
            GroupName = string.Empty;
            RestPosition = RestPosition.None;
            IsHidden = false;
        }

        [JsonConstructor]
        internal DisplayData(User user, ushort hairSprite, byte hairColor, byte bodySprite, BodyColor bodyColor, byte faceSprite, ushort armorSprite2, NameTagStyle nameTagStyle, string groupName)
        {
            User = user;
            HairSprite = hairSprite;
            HairColor = hairColor;
            BodySprite = bodySprite;
            BodyColor = bodyColor;
            FaceSprite = faceSprite;
            ArmorSprite2 = armorSprite2;
            NameTagStyle = nameTagStyle;
            GroupName = groupName;
        }
    }
}
