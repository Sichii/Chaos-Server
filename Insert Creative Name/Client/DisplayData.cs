using System;

namespace Chaos
{
    internal sealed class DisplayData
    {
        private readonly Objects.User User;

        //Base
        internal ushort HairSprite { get; set; }
        internal byte HairColor { get; set; }
        internal byte BodySprite { get; set; }
        internal byte BodyColor { get; set; }
        internal byte FaceSprite { get; set; }

        //Head
        internal ushort HeadSprite => User.Equipment[(byte)EquipmentSlot.OverHelm]?.Sprite ?? User.Equipment[(byte)EquipmentSlot.Helmet]?.Sprite ?? HairSprite;
        internal byte HeadColor => User.Equipment[(byte)EquipmentSlot.OverHelm]?.Color ?? User.Equipment[(byte)EquipmentSlot.Helmet]?.Color ?? HairColor;

        //Body
        internal ushort ArmorSprite1 => User.Equipment[(byte)EquipmentSlot.Armor]?.Sprite ?? 0;
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
        internal byte LanternSize { get; set; }
        internal byte NameTagStyle { get; set; }
        internal string GroupName { get; set; }
        internal byte RestPosition { get; set; }
        internal bool IsHidden { get; set; }

        /// <summary>
        /// Object containing the methods and information that is used to display the <c>User</c>.
        /// </summary>
        /// <param name="user"><c>User</c> to be displayed.</param>
        /// <param name="hairSprite">Base sprite for the hair of the user.</param>
        /// <param name="hairColor">Base color value for the hair of the user.</param>
        /// <param name="bodySprite">Base body value for the user.</param>
        internal DisplayData(Objects.User user, ushort hairSprite, byte hairColor, byte bodySprite)
        {
            User = user;
            HairSprite = hairSprite;
            HairColor = hairColor;
            BodySprite = bodySprite;
            BodyColor = 1;
            FaceSprite = 1;
        }
    }
}
