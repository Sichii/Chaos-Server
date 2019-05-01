// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.MapTile
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.Drawing
{
    public class MapTile
    {
        private ushort floor;
        private ushort leftWall;
        private ushort rightWall;

        public ushort RightWall
        {
            get
            {
                return rightWall;
            }
            set
            {
                rightWall = value;
            }
        }

        public ushort LeftWall
        {
            get
            {
                return leftWall;
            }
            set
            {
                leftWall = value;
            }
        }

        public ushort FloorTile
        {
            get
            {
                return floor;
            }
            set
            {
                floor = value;
            }
        }

        public MapTile(ushort floor, ushort leftWall, ushort rightWall)
        {
            this.floor = floor;
            this.leftWall = leftWall;
            this.rightWall = rightWall;
        }

        public override string ToString()
        {
            return "{Floor = " + floor.ToString() + ", Left Wall = " + leftWall.ToString() + ", Right Wall = " + rightWall.ToString() + "}";
        }
    }
}
