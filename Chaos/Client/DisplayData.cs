// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using Newtonsoft.Json;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class DisplayData
    {
        internal User User { private get; set; }

        //Base
        [JsonProperty]
        internal ushort HairSprite { get; set; }
        [JsonProperty]
        internal byte HairColor { get; set; }
        [JsonProperty]
        internal BodySprite BodySprite { get; set; }
        [JsonProperty]
        internal BodyColor BodyColor { get; set; }
        [JsonProperty]
        internal byte FaceSprite { get; set; }

        //Head
        internal ushort HeadSprite => User.Equipment[EquipmentSlot.OverHelm]?.ItemSprite.DisplaySprite ?? User.Equipment[EquipmentSlot.Helmet]?.ItemSprite.DisplaySprite ?? HairSprite;
        internal byte HeadColor => User.Equipment[EquipmentSlot.OverHelm]?.Color ?? User.Equipment[EquipmentSlot.Helmet]?.Color ?? HairColor;

        //Body
        internal ushort ArmorSprite1 => User.Equipment[EquipmentSlot.Armor]?.ItemSprite.DisplaySprite ?? 0;
        internal ushort ArmorSprite2 => ArmorSprite1;
        internal ushort OvercoatSprite => User.Equipment[EquipmentSlot.Overcoat]?.ItemSprite.DisplaySprite ?? 0;
        internal byte OvercoatColor => User.Equipment[EquipmentSlot.Overcoat]?.Color ?? 0;

        //Boots
        internal byte BootsSprite => (byte)(User.Equipment[EquipmentSlot.Boots]?.ItemSprite.DisplaySprite ?? 0);
        internal byte BootsColor => User.Equipment[EquipmentSlot.Boots]?.Color ?? 0;

        //Hands
        internal byte ShieldSprite => (byte)(User.Equipment[EquipmentSlot.Shield]?.ItemSprite.DisplaySprite ?? 0);
        internal ushort WeaponSprite => (byte)(User.Equipment[EquipmentSlot.Weapon]?.ItemSprite.DisplaySprite ?? 0);

        //Accessories
        internal byte AccessoryColor1 => User.Equipment[EquipmentSlot.Accessory1]?.Color ?? 0;
        internal byte AccessoryColor2 => User.Equipment[EquipmentSlot.Accessory2]?.Color ?? 0;
        internal byte AccessoryColor3 => User.Equipment[EquipmentSlot.Accessory3]?.Color ?? 0;
        internal ushort AccessorySprite1 => User.Equipment[EquipmentSlot.Accessory1]?.ItemSprite.DisplaySprite ?? 0;
        internal ushort AccessorySprite2 => User.Equipment[EquipmentSlot.Accessory2]?.ItemSprite.DisplaySprite ?? 0;
        internal ushort AccessorySprite3 => User.Equipment[EquipmentSlot.Accessory3]?.ItemSprite.DisplaySprite ?? 0;

        //Other
        internal LanternSize LanternSize => LanternSize.None;
        [JsonProperty]
        internal NameTagStyle NameTagStyle { get; set; }
        [JsonProperty]
        internal string GroupName { get; set; }
        internal RestPosition RestPosition => RestPosition.None;
        internal bool IsHidden => false;

        /// <summary>
        /// Base constructor for an object containing all data used to display a new user.
        /// </summary>
        /// <param name="user">User to be displayed.</param>
        /// <param name="hairSprite">Base sprite for the hair of the user.</param>
        /// <param name="hairColor">Base color value for the hair of the user.</param>
        /// <param name="bodySprite">Base body value for the user.</param>
        internal DisplayData(User user, ushort hairSprite, byte hairColor, BodySprite bodySprite)
        {
            User = user;
            HairSprite = hairSprite;
            HairColor = hairColor;
            BodySprite = bodySprite;
            BodyColor = BodyColor.White;
            FaceSprite = 1;
            NameTagStyle = NameTagStyle.NeutralHover;
            GroupName = string.Empty;
        }

        /// <summary>
        /// Json & Master constructor for an object containing all data used to display an existing user.
        /// </summary>
        [JsonConstructor]
        internal DisplayData(ushort hairSprite, byte hairColor, BodySprite bodySprite, BodyColor bodyColor, byte faceSprite, ushort armorSprite2, NameTagStyle nameTagStyle, string groupName)
        {
            HairSprite = hairSprite;
            HairColor = hairColor;
            BodySprite = bodySprite;
            BodyColor = bodyColor;
            FaceSprite = faceSprite;
            NameTagStyle = nameTagStyle;
            GroupName = groupName;
        }
    }
}
