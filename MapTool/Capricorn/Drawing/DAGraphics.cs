// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.DAGraphics
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using Capricorn.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Capricorn.Drawing
{
    public class DAGraphics
    {

        public static Bitmap RenderImage(HPFImage hpf, Palette256 palette)
        {
            return DAGraphics.SimpleRender(hpf.Width, hpf.Height, hpf.RawData, palette, ImageType.HPF);
        }

        public static Bitmap RenderImage(EPFFrame epf, Palette256 palette)
        {
            if (epf.Width != 0 || epf.Height != 0)
                return SimpleRender(epf.Width, epf.Height, epf.RawData, palette, ImageType.EPF);
            return new Bitmap(1, 1);
        }

        public static Bitmap RenderImage(MPFFrame mpf, Palette256 palette)
        {
            return DAGraphics.SimpleRender(mpf.Width, mpf.Height, mpf.RawData, palette, ImageType.MPF);
        }

        public static Bitmap RenderTile(byte[] tileData, Palette256 palette)
        {
            return DAGraphics.SimpleRender(56, 27, tileData, palette, ImageType.Tile);
        }
        
        private static unsafe Bitmap SimpleRender(int width, int height, byte[] data, Palette256 palette, ImageType type)
        {
            Bitmap bitmap = new Bitmap(width, height);
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly, bitmap.PixelFormat);
            for (int index1 = 0; index1 < bitmapdata.Height; ++index1)
            {
                byte* numPtr = (byte*) ((Int32) (void*) bitmapdata.Scan0 + index1 * bitmapdata.Stride);
                for (int index2 = 0; index2 < bitmapdata.Width; ++index2)
                {
                    int index3 = type != ImageType.EPF
                        ? data[index1 * width + index2]
                        : data[index2 * height + index1];
                    if (index3 > 0)
                    {
                        if (bitmapdata.PixelFormat == PixelFormat.Format32bppArgb)
                        {
                            numPtr[index2 * 4] = palette[index3].B;
                            numPtr[index2*4 + 1] = palette[index3].G;
                            numPtr[index2*4 + 2] = palette[index3].R;
                            numPtr[index2*4 + 3] = palette[index3].A;
                        }
                        else if (bitmapdata.PixelFormat == PixelFormat.Format24bppRgb)
                        {
                            numPtr[index2 * 3] = palette[index3].B;
                            numPtr[index2*3 + 1] = palette[index3].G;
                            numPtr[index2*3 + 2] = palette[index3].R;
                        }
                        else if (bitmapdata.PixelFormat == PixelFormat.Format16bppRgb555)
                        {
                            ushort num =
                                (ushort)
                                    (((palette[index3].R & 248) << 7) + ((palette[index3].G & 248) << 2) +
                                     (palette[index3].B >> 3));
                            numPtr[index2 * 2] = (byte)(num % 256U);
                            numPtr[index2*2 + 1] = (byte)(num / 256U);
                        }
                        else if (bitmapdata.PixelFormat == PixelFormat.Format16bppRgb565)
                        {
                            ushort num =
                                (ushort)
                                    (((palette[index3].R & 248) << 8) + ((palette[index3].G & 252) << 3) +
                                     (palette[index3].B >> 3));
                            numPtr[index2 * 2] = (byte)(num % 256U);
                            numPtr[index2*2 + 1] = (byte)(num / 256U);
                        }
                    }
                }
            }
            bitmap.UnlockBits(bitmapdata);
            if (type == ImageType.EPF)
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
            return bitmap;
        }

        public static int displaywidth = 560;
        public static int displayheight = 480;

        public static Bitmap RenderMap(MAPFile map, Tileset tiles, PaletteTable tileTable, PaletteTable wallTable,
            DATArchive wallSource)
        {
            int num1 = 256;
            int num2 = 96;
            Bitmap bitmap1 = new Bitmap(56*map.Width, 27*(map.Height + 1) + num1 + num2);
            Graphics graphics = Graphics.FromImage(bitmap1);
            int num3 = bitmap1.Width / 2 - 1 - 28 + 1;
            int num4 = num1;
            Dictionary<int, Bitmap> dictionary1 = new Dictionary<int, Bitmap>();
            for (int index1 = 0; index1 < map.Height; ++index1)
            {
                for (int index2 = 0; index2 < map.Width; ++index2)
                {
                    int key = map[index2, index1].FloorTile;
                    if (key > 0)
                        --key;
                    if (!dictionary1.ContainsKey(key))
                    {
                        Bitmap bitmap2 = DAGraphics.RenderTile(tiles[key], tileTable[key + 2]);
                        dictionary1.Add(key, bitmap2);
                    }
                    graphics.DrawImageUnscaled(dictionary1[key], num3 + index2*56/2, num4 + index2*28/2);
                }
                num3 -= 28;
                num4 += 14;
            }
            int num5 = bitmap1.Width/ 2 - 1 - 28 + 1;
            int num6 = num1;
            Dictionary<int, Bitmap> dictionary2 = new Dictionary<int, Bitmap>();
            for (int index1 = 0; index1 < map.Height; ++index1)
            {
                for (int index2 = 0; index2 < map.Width; ++index2)
                {
                    int key1 = map[index2, index1].LeftWall;
                    if (!dictionary2.ContainsKey(key1))
                    {
                        Bitmap bitmap2 =
                            DAGraphics.RenderImage(
                                HPFImage.FromArchive("stc" + key1.ToString().PadLeft(5, '0') + ".hpf", true, wallSource),
                                wallTable[key1 + 1]);
                        dictionary2.Add(key1, bitmap2);
                    }
                    if (key1%10000 > 1)
                        graphics.DrawImageUnscaled(dictionary2[key1], num5 + index2*56/2,
                            num6 + (index2 + 1)*28/2 - dictionary2[key1].Height + 14);
                    int key2 = map[index2, index1].RightWall;
                    if (!dictionary2.ContainsKey(key2))
                    {
                        Bitmap bitmap2 =
                            DAGraphics.RenderImage(
                                HPFImage.FromArchive("stc" + key2.ToString().PadLeft(5, '0') + ".hpf", true, wallSource),
                                wallTable[key2 + 1]);
                        dictionary2.Add(key2, bitmap2);
                    }
                    if (key2%10000 > 1)
                        graphics.DrawImageUnscaled(dictionary2[key2], num5 + (index2 + 1)*56/2,
                            num6 + (index2 + 1)*28/2 - dictionary2[key2].Height + 14);
                }
                num5 -= 28;
                num6 += 14;
            }
            //SolidBrush solidBrush = new SolidBrush(Color.White);
            //graphics.DrawString(map.Name.ToUpper(), new Font("04b03b", 6f, FontStyle.Regular), (Brush)solidBrush, 16f, 16f);
            //graphics.DrawString("LOD" + map.ID.ToString() + ".MAP", new Font("04b03b", 6f, FontStyle.Regular), (Brush)solidBrush, 16f, 26f);
            //graphics.DrawString(map.Width.ToString() + "x" + map.Height.ToString() + " TILES", new Font("04b03b", 6f, FontStyle.Regular), (Brush)solidBrush, 16f, 36f);
            //solidBrush.Dispose();
            graphics.Dispose();
            return bitmap1;
        }
    }
}
