/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
// Copyright 2009, 2010 Matvei Stefarov <me@matvei.org>
/*
This generator was developed by Neko_baron.

Ideas, concepts, and code were used from the following two sources:
1) Isaac McGarvey's 'perlin noise generator' code
2) http://www.lighthouse3d.com/opengl/terrain/index.php3?introduction

*/
using System;
namespace MCGalaxy {
	
    public static class TreeGen {

        public static void AddTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte height = (byte)Rand.Next(5, 8);
            short top = (short)(height - Rand.Next(2, 4));
            ushort xxx, yyy, zzz;
            for (ushort yy = 0; yy < top + height - 1; yy++)
            {
                if (overwrite || Lvl.GetTile(x, (ushort)(y + yy), z) == Block.air || (y + yy == y && Lvl.GetTile(x, (ushort)(y + yy), z) == Block.shrub))
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, (ushort)(y + yy), z, Block.trunk);
                        else Lvl.Blockchange(p, x, (ushort)(y + yy), z, Block.trunk);
                    else Lvl.SetTile(x, (ushort)(y + yy), z, Block.trunk);
            }


            for (short xx = (short)-top; xx <= top; ++xx)
            {
                for (short yy = (short)-top; yy <= top; ++yy)
                {
                    for (short zz = (short)-top; zz <= top; ++zz)
                    {
                        short Dist = (short)(Math.Sqrt(xx * xx + yy * yy + zz * zz));
                        if (Dist < top + 1)
                        {
                            if (Rand.Next((int)(Dist)) < 2)
                            {
                                try
                                {
                                    xxx = (ushort)(x + xx);
                                    yyy = (ushort)(y + yy + height);
                                    zzz = (ushort)(z + zz);

                                    if ((xxx != x || zzz != z || yy >= top - 1) && (overwrite || Lvl.GetTile(xxx, yyy, zzz) == Block.air))
                                        if (blockChange)
                                            if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                            else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                                        else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }

        public static void AddNotchTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte dist, tile;
            byte height = (byte)Rand.Next(3, 7);
            byte top = (byte)(height - 2);
            short xx, yy, zz;
            ushort xxx, yyy, zzz;
            for (yy = 0; yy <= height; yy++)
            {
                yyy = (ushort)(y + yy);
                tile = Lvl.GetTile(x, yyy, z);
                if (overwrite || tile == Block.air || (yyy == y && tile == Block.shrub))
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, yyy, z, Block.trunk);
                        else Lvl.Blockchange(p, x, yyy, z, Block.trunk);
                    else Lvl.SetTile(x, yyy, z, Block.trunk);
            }

            for (yy = top; yy <= height + 1; yy++)
            {
                dist = yy > height - 1 ? (byte)1 : (byte)2;
                for (xx = (short)-dist; xx <= dist; xx++)
                {
                    for (zz = (short)-dist; zz <= dist; zz++)
                    {
                        xxx = (ushort)(x + xx);
                        yyy = (ushort)(y + yy);
                        zzz = (ushort)(z + zz);
                        tile = Lvl.GetTile(xxx, yyy, zzz);
                        //Server.s.Log(String.Format("{0} {1} {2}", xxx, yyy, zzz));

                        if ((xxx == x && zzz == z && yy <= height) || (!overwrite && tile != Block.air))
                            continue;

                        if (Math.Abs(xx) == dist && Math.Abs(zz) == dist)
                        {
                            if (yy > height)
                                continue;

                            if (Rand.Next(2) == 0)
                            {
                                if (blockChange)
                                    if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                    else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                                else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                            }
                        }
                        else
                        {
                            if (blockChange)
                                if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                            else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                        }
                    }
                }
            }
        }

        public static void AddNotchBigTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            throw new NotImplementedException();
        }

        public static void AddNotchPineTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            throw new NotImplementedException();
            //byte height = (byte)Rand.Next(7, 12);
        }

        public static void AddNotchSwampTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte dist, tile;
            byte height = (byte)Rand.Next(4, 8);
            byte top = (byte)(height - 2);
            short xx, yy, zz;
            ushort xxx, yyy, zzz;
            for (yy = 0; yy <= height; yy++)
            {
                yyy = (ushort)(y + yy);
                tile = Lvl.GetTile(x, yyy, z);
                if (overwrite || tile == Block.air || (yyy == y && tile == Block.shrub))
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, yyy, z, Block.trunk);
                        else Lvl.Blockchange(p, x, yyy, z, Block.trunk);
                    else Lvl.SetTile(x, yyy, z, Block.trunk);
            }

            for (yy = top; yy <= height + 1; yy++)
            {
                dist = yy > height - 1 ? (byte)2 : (byte)3;
                for (xx = (short)-dist; xx <= dist; xx++)
                {
                    for (zz = (short)-dist; zz <= dist; zz++)
                    {
                        xxx = (ushort)(x + xx);
                        yyy = (ushort)(y + yy);
                        zzz = (ushort)(z + zz);
                        tile = Lvl.GetTile(xxx, yyy, zzz);
                        //Server.s.Log(String.Format("{0} {1} {2}", xxx, yyy, zzz));

                        if ((xxx == x && zzz == z && yy <= height) || (!overwrite && tile != Block.air))
                            continue;

                        if (Math.Abs(xx) == dist && Math.Abs(zz) == dist)
                        {
                            if (yy > height)
                                continue;

                            if (Rand.Next(2) == 0)
                            {
                                if (blockChange)
                                    if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                    else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                                else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                            }
                        }
                        else
                        {
                            if (blockChange)
                                if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                            else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                        }
                    }
                }
            }
        }

        public static void AddCactus(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte height = (byte)Rand.Next(3, 6);
            ushort yy;

            for (yy = 0; yy <= height; yy++)
            {
                if (overwrite || Lvl.GetTile(z, (ushort)(y + yy), z) == Block.air)
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, (ushort)(y + yy), z, Block.green);
                        else Lvl.Blockchange(p, x, (ushort)(y + yy), z, Block.green);
                    else Lvl.SetTile(x, (ushort)(y + yy), z, Block.green);
            }

            int inX = 0, inZ = 0;

            switch (Rand.Next(1, 3))
            {
                case 1: inX = -1; break;
                case 2:
                default: inZ = -1; break;
            }

            for (yy = height; yy <= Rand.Next(height + 2, height + 5); yy++)
            {
                if (overwrite || Lvl.GetTile((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ)) == Block.air)
                    if (blockChange)
                        if (p == null) Lvl.Blockchange((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ), Block.green);
                        else Lvl.Blockchange(p, (ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ), Block.green);
                    else Lvl.SetTile((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ), Block.green);
            }
            for (yy = height; yy <= Rand.Next(height + 2, height + 5); yy++)
            {
                if (overwrite || Lvl.GetTile((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ)) == Block.air)
                    if (blockChange)
                        if (p == null) Lvl.Blockchange((ushort)(x - inX), (ushort)(y + yy), (ushort)(z - inZ), Block.green);
                        else Lvl.Blockchange(p, (ushort)(x - inX), (ushort)(y + yy), (ushort)(z - inZ), Block.green);
                    else Lvl.SetTile((ushort)(x - inX), (ushort)(y + yy), (ushort)(z - inZ), Block.green);
            }
        }

        public static bool TreeCheck(Level Lvl, ushort x, ushort z, ushort y, short dist)         //return true if tree is near
        {
            byte foundTile;
            for (short xx = (short)-dist; xx <= +dist; ++xx)
            {
                for (short yy = (short)-dist; yy <= +dist; ++yy)
                {
                    for (short zz = (short)-dist; zz <= +dist; ++zz)
                    {
                        foundTile = Lvl.GetTile((ushort)(x + xx), (ushort)(z + zz), (ushort)(y + yy));
                        if (foundTile == Block.trunk || foundTile == Block.green)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}