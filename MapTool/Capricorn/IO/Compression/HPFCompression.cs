// Decompiled with JetBrains decompiler
// Type: Capricorn.IO.Compression.HPFCompression
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using System;
using System.IO;

namespace Capricorn.IO.Compression
{
    public class HPFCompression
    {
        public static byte[] Decompress(string file)
        {
            uint num1 = 7U;
            uint num2 = 0U;
            uint num3 = 0U;
            uint num4 = 0U;
            byte[] numArray1 = File.ReadAllBytes(file);
            byte[] numArray2 = new byte[numArray1.Length * 10];
            uint[] numArray3 = new uint[256];
            uint[] numArray4 = new uint[256];
            byte[] numArray5 = new byte[513];
            for (uint index = 0U; index < 256U; ++index)
            {
                numArray3[(Int32)index] = (uint)(2 * (int)index + 1);
                numArray4[(Int32)index] = (uint)(2 * (int)index + 2);
                numArray5[(Int32)(uint)((int)index * 2 + 1)] = (byte)index;
                numArray5[(Int32)(uint)((int)index * 2 + 2)] = (byte)index;
            }
            while ((int)num2 != 256)
            {
                uint num5;
                for (num5 = 0U; num5 <= byte.MaxValue; num5 = (numArray1[(Int32)(uint)(4 + (int)num3 - 1)] & 1 << (int)num1) == 0 ? numArray3[(Int32)num5] : numArray4[(Int32)num5])
                {
                    if ((int)num1 == 7)
                    {
                        ++num3;
                        num1 = 0U;
                    }
                    else
                        ++num1;
                }
                uint num6 = num5;
                for (uint index = numArray5[(Int32)num5]; (int)num6 != 0 && (int)index != 0; index = numArray5[(Int32)num6])
                {
                    uint num7 = numArray5[(Int32)index];
                    uint num8 = numArray3[(Int32)num7];
                    if ((int)num8 == (int)index)
                    {
                        num8 = numArray4[(Int32)num7];
                        numArray4[(Int32)num7] = num6;
                    }
                    else
                        numArray3[(Int32)num7] = num6;
                    if ((int)numArray3[(Int32)index] == (int)num6)
                        numArray3[(Int32)index] = num8;
                    else
                        numArray4[(Int32)index] = num8;
                    numArray5[(Int32)num6] = (byte)num7;
                    numArray5[(Int32)num8] = (byte)index;
                    num6 = num7;
                }
                num2 = num5 + 4294967040U;
                if ((int)num2 != 256)
                {
                    numArray2[(Int32)num4] = (byte)num2;
                    ++num4;
                }
            }
            byte[] numArray6 = new byte[(Int32)num4];
            Buffer.BlockCopy(numArray2, 0, numArray6, 0, (int)num4);
            return numArray6;
        }

        public static byte[] Decompress(byte[] hpfBytes)
        {
            uint num1 = 7U;
            uint num2 = 0U;
            uint num3 = 0U;
            uint num4 = 0U;
            byte[] numArray1 = new byte[hpfBytes.Length * 10];
            uint[] numArray2 = new uint[256];
            uint[] numArray3 = new uint[256];
            byte[] numArray4 = new byte[513];
            for (uint index = 0U; index < 256U; ++index)
            {
                numArray2[(Int32)index] = (uint)(2 * (int)index + 1);
                numArray3[(Int32)index] = (uint)(2 * (int)index + 2);
                numArray4[(Int32)(uint)((int)index * 2 + 1)] = (byte)index;
                numArray4[(Int32)(uint)((int)index * 2 + 2)] = (byte)index;
            }
            while ((int)num2 != 256)
            {
                uint num5;
                for (num5 = 0U; num5 <= byte.MaxValue; num5 = (hpfBytes[(Int32)(uint)(4 + (int)num3 - 1)] & 1 << (int)num1) == 0 ? numArray2[(Int32)num5] : numArray3[(Int32)num5])
                {
                    if ((int)num1 == 7)
                    {
                        ++num3;
                        num1 = 0U;
                    }
                    else
                        ++num1;
                }
                uint num6 = num5;
                for (uint index = numArray4[(Int32)num5]; (int)num6 != 0 && (int)index != 0; index = numArray4[(Int32)num6])
                {
                    uint num7 = numArray4[(Int32)index];
                    uint num8 = numArray2[(Int32)num7];
                    if ((int)num8 == (int)index)
                    {
                        num8 = numArray3[(Int32)num7];
                        numArray3[(Int32)num7] = num6;
                    }
                    else
                        numArray2[(Int32)num7] = num6;
                    if ((int)numArray2[(Int32)index] == (int)num6)
                        numArray2[(Int32)index] = num8;
                    else
                        numArray3[(Int32)index] = num8;
                    numArray4[(Int32)num6] = (byte)num7;
                    numArray4[(Int32)num8] = (byte)index;
                    num6 = num7;
                }
                num2 = num5 + 4294967040U;
                if ((int)num2 != 256)
                {
                    numArray1[(Int32)num4] = (byte)num2;
                    ++num4;
                }
            }
            byte[] numArray5 = new byte[(Int32)num4];
            Buffer.BlockCopy(numArray1, 0, numArray5, 0, (int)num4);
            return numArray5;
        }
    }
}
