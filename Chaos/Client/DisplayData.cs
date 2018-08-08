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
using System.Collections.Generic;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptOut)]
    internal sealed class DisplayData
    {
        internal static List<ushort> HairSprites = new List<ushort>()
        {
            0,
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
            161, 253, 254, 255, 263, 264, 265, 266, 313, 314, 321, 324, 325, 326, 327, 333, 342, 343, 344, 345,
            346, 347, 349, 383, 392, 397, 411, 412, 433, 435, 437, 438, 440, 441, 447, 448, 449, 459, 460, 461,
            476, 482, 483
        };

        [JsonProperty]
        internal User User { get; set; }

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
        internal DisplayData(User user, ushort hairSprite, byte hairColor, BodySprite bodySprite)
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
        internal DisplayData(User user, ushort hairSprite, byte hairColor, BodySprite bodySprite, BodyColor bodyColor, byte faceSprite, ushort armorSprite2, NameTagStyle nameTagStyle, string groupName)
        {
            User = user;
            HairSprite = hairSprite;
            HairColor = hairColor;
            BodySprite = bodySprite;
            BodyColor = bodyColor;
            FaceSprite = faceSprite;
            //ArmorSprite2 = armorSprite2;
            NameTagStyle = nameTagStyle;
            GroupName = groupName;
        }
    }
}
