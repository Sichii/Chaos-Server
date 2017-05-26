using System;

namespace Insert_Creative_Name
{
    [Serializable]
    internal sealed class DisplayData
    {
        //Head
        internal ushort HeadSprite;
        internal byte HeadColor;
        internal byte FaceSprite;

        //Body
        internal byte BodySprite;
        internal byte BodyColor;
        internal ushort ArmorSprite1;
        internal ushort ArmorSprite2;
        internal ushort OvercoatSprite;
        internal byte OvercoatColor;

        //Boots
        internal byte BootsSprite;
        internal byte BootsColor;

        //Hands
        internal byte ShieldSprite;
        internal ushort WeaponSprite;

        //Accessories
        internal byte AccessoryColor1;
        internal byte AccessoryColor2;
        internal byte AccessoryColor3;
        internal ushort AccessorySprite1;
        internal ushort AccessorySprite2;
        internal ushort AccessorySprite3;

        //Other
        internal byte LanternSize;
        internal byte NameTagStyle;
        internal string GroupName;
        internal bool RestPosition;
        internal bool IsHidden;
    }
}
