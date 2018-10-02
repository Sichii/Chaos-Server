// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.MapTile
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.Drawing
{
    public class MapTile
    {
        public ushort RightWall { get; set; }

        public ushort LeftWall { get; set; }

        public ushort FloorTile { get; set; }

        public MapTile(ushort floor, ushort leftWall, ushort rightWall)
        {
            FloorTile = floor;
            LeftWall = leftWall;
            RightWall = rightWall;
        }

        public override string ToString() => "{Floor = " + FloorTile.ToString() + ", Left Wall = " + LeftWall.ToString() + ", Right Wall = " + RightWall.ToString() + "}";
    }
}
