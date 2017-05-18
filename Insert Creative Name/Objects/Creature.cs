using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name.Objects
{
    internal class Creature : VisibleObject
    {
        internal Direction Direction { get; set; }
        internal DateTime LastCurse { get; set; }
        internal DateTime LastFas { get; set; }
        internal DateTime LastAite { get; set; }
        internal DateTime LastStep { get; set; }
        internal double CurseLength { get; set; }
        internal double FasLength { get; set; }
        internal double AiteLength { get; set; }
        internal Dictionary<ushort, DateTime> LastAnimation { get; set; }
        internal Dictionary<ushort, DateTime> LastForeignAnimation { get; set; }
        private byte healthPercent;
        internal byte Type { get; set; }
        internal string CurseType { get; set; }
        internal DateTime LastDion { get; set; }
        internal string DionType { get; set; }
        internal double dionLength;
        internal bool doesDion = false;
        internal int Clicked = 0;

        internal double DionLength
        {
            get { return dionLength; }
            set
            {
                doesDion = true;
                dionLength = value;
            }
        }

        internal byte HealthPercent
        {
            get { return (byte)(healthPercent > 100 ? 100 : healthPercent); }
            set { healthPercent = value; }
        }
        internal bool IsDioned
        {
            get { return DateTime.UtcNow.Subtract(LastDion).TotalSeconds < DionLength; }
        }

        internal bool IsCursed
        {
            get { return DateTime.UtcNow.Subtract(LastCurse).TotalSeconds < CurseLength; }
        }

        internal bool IsFasNadured
        {
            get { return DateTime.UtcNow.Subtract(LastFas).TotalSeconds < FasLength; }
        }

        internal bool IsNaomhAited
        {
            get { return DateTime.UtcNow.Subtract(LastAite).TotalSeconds < AiteLength; }
        }

        internal bool IsSuained
        {
            get
            {
                if (LastAnimation.ContainsKey(40))
                    return DateTime.UtcNow.Subtract(LastAnimation[40]).TotalSeconds < 1.5;
                return false;
            }
        }

        internal bool IsFrosted
        {
            get
            {   //235 is frost arrow, 377 is frost strike
                if (LastAnimation.ContainsKey(235) && DateTime.UtcNow.Subtract(LastAnimation[235]).TotalSeconds < 2)
                    return true;
                if (LastAnimation.ContainsKey(377) && DateTime.UtcNow.Subtract(LastAnimation[377]).TotalSeconds < 1)
                    return true;
                return false;
            }
        }

        internal bool IsPramhed
        {
            get
            {
                if (LastAnimation.ContainsKey(117) && DateTime.UtcNow.Subtract(LastAnimation[117]).TotalSeconds < 1.5)
                    return true;
                if (LastAnimation.ContainsKey(32) && DateTime.UtcNow.Subtract(LastAnimation[32]).TotalSeconds < 3.0)
                    return true;
                return false;
            }
        }

        internal bool IsPoisoned
        {
            get
            {
                if (LastAnimation.ContainsKey(25) && DateTime.UtcNow.Subtract(LastAnimation[25]).TotalSeconds < 1.5)
                    return true;
                if (LastAnimation.ContainsKey(247) && DateTime.UtcNow.Subtract(LastAnimation[247]).TotalSeconds < 3)
                    return true;
                if (LastAnimation.ContainsKey(295) && DateTime.UtcNow.Subtract(LastAnimation[295]).TotalSeconds < 3)
                    return true;
                return false;
            }
        }

        internal Creature(uint id, string name, ushort sprite, byte type, Point point, Map map, Direction direction)
            : base(id, name, sprite, point, map)
        {
            Direction = direction;
            LastAnimation = new Dictionary<ushort, DateTime>();
            LastForeignAnimation = new Dictionary<ushort, DateTime>();
            healthPercent = 100;
            Type = type;
            LastStep = DateTime.UtcNow;
            LastCurse = DateTime.MinValue;
            LastFas = DateTime.MinValue;
            LastAite = DateTime.MinValue;
            LastDion = DateTime.MinValue;
            LastAnimation = new Dictionary<ushort, DateTime>();
            LastForeignAnimation = new Dictionary<ushort, DateTime>();
            CurseType = "";
            DionType = "";
        }
    }
}
