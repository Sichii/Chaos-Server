using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal class User : Creature
    {
        internal ushort HeadSprite { get; set; }
        internal byte BodySprite { get; set; }
        internal ushort ArmorSprite1 { get; set; }
        internal ushort ArmorSprite2 { get; set; }
        internal byte BootsSprite { get; set; }
        internal byte ShieldSprite { get; set; }
        internal ushort WeaponSprite { get; set; }
        internal byte HeadColor { get; set; }
        internal byte BootsColor { get; set; }
        internal byte AccessoryColor1 { get; set; }
        internal ushort AccessorySprite1 { get; set; }
        internal byte AccessoryColor2 { get; set; }
        internal ushort AccessorySprite2 { get; set; }
        internal byte AccessoryColor3 { get; set; }
        internal ushort AccessorySprite3 { get; set; }
        internal byte LanternSize { get; set; }
        internal byte RestPosition { get; set; }
        internal ushort OvercoatSprite { get; set; }
        internal byte OvercoatColor { get; set; }
        internal byte BodyColor { get; set; }
        internal bool IsHidden { get; set; }
        internal byte FaceSprite { get; set; }
        internal byte NameTagStyle { get; set; }
        internal string GroupName { get; set; }
        internal bool NeedsHeal { get; set; }
        internal DateTime LastDiaArm { get; set; }


        internal new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        internal bool IsArmachd
        {
            get
            {
                if (LastAnimation.ContainsKey(20))
                    return (DateTime.UtcNow.Subtract(LastDiaArm).TotalMinutes < 30 || DateTime.UtcNow.Subtract(LastAnimation[20]).TotalMinutes < 2.5);
                return false;
            }
        }

        internal bool IsSkulled
        {
            get
            {
                if (LastAnimation.ContainsKey(24))
                    return DateTime.UtcNow.Subtract(LastAnimation[24]).TotalSeconds < 2;
                return false;
            }
        }

        internal User(uint id, string name, Point point, Map map, Direction direction)
          : base(id, name, 0, 4, point, map, direction)
        {
            Type = 4;
            LastDiaArm = DateTime.MinValue;
        }

        internal void ResetDisplayData()
        {
            ArmorSprite1 = 0;
            BootsSprite = 0;
            ArmorSprite2 = 0;
            ShieldSprite = 0;
            WeaponSprite = 0;
            HeadColor = 0;
            BootsColor = 0;
            AccessoryColor1 = 0;
            AccessorySprite1 = 0;
            AccessoryColor2 = 0;
            AccessorySprite2 = 0;
            AccessoryColor2 = 0;
            AccessorySprite2 = 0;
            LanternSize = 0;
            RestPosition = 0;
            OvercoatSprite = 0;
            OvercoatColor = 0;
        }
    }
}
