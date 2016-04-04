/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System;

namespace MCGalaxy.BlockPhysics {
    
    public static class ExtraInfoPhysics {
        
        public static bool DoDoorsOnly(Level lvl, Check C, Random rand) {
            string info = C.data as string;
            if (info == null) return true;
            if (!info.Contains("wait") && lvl.blocks[C.b] == Block.air)
                C.data = "";

            bool wait = false, door = false;
            int waitTime = 0;
            string[] parts = info.Split(' ');
            for (int i = 0; i < parts.Length; i++) {
                if (i % 2 != 0) continue;
                
                switch (parts[i]) {
                    case "wait":
                        waitTime = int.Parse(parts[i + 1]);
                        wait = true; break;
                    case "door":
                        door = true; break;
                }
            }
            if (!wait)
                return false;
            
            if (door && C.time < 2) {
                // TODO: perhaps do proper bounds checking
                Checktdoor(lvl, lvl.IntOffset(C.b, -1, 0, 0));
                Checktdoor(lvl, lvl.IntOffset(C.b, 1, 0, 0));
                Checktdoor(lvl, lvl.IntOffset(C.b, 0, -1, 0));
                Checktdoor(lvl, lvl.IntOffset(C.b, 0, 1, 0));
                Checktdoor(lvl, lvl.IntOffset(C.b, 0, 0, -1));
                Checktdoor(lvl, lvl.IntOffset(C.b, 0, 0, 1));
            }

            if (C.time > waitTime) {
                int waitIndex = info.IndexOf("wait ");
                C.data =
                    info.Substring(0, waitIndex) +
                    info.Substring(info.IndexOf(' ', waitIndex + 5) + 1);
                return false;
            }
            C.time++;
            return true;
        }
        
        static void Checktdoor(Level lvl, int index) {
            if (index < 0 || index >= lvl.blocks.Length) return;
            byte block = lvl.blocks[index];
            
            if (Block.tDoor(block)) {
                lvl.AddUpdate(index, Block.air, false,
                              "wait 10 door 1 revert " + block.ToString());
            }
        }
        
        public static bool DoComplex(Level lvl, Check C) {
            string info = C.data as string;
            if (info == null) return true;
            if (!info.Contains("wait") && lvl.blocks[C.b] == Block.air)
                C.data = "";

            ExtraInfoArgs args = default(ExtraInfoArgs);
            string[] parts = info.Split(' ');
            
            for (int i = 0; i < parts.Length; i++) {
                if (i % 2 != 0) continue;
                
                switch (parts[i]) {
                    case "wait":
                        args.WaitTime = int.Parse(parts[i + 1]);
                        args.Wait = true; break;
                    case "drop":
                        args.DropNum = int.Parse(parts[i + 1]);
                        args.Drop = true; break;
                    case "dissipate":
                        args.DissipateNum = int.Parse(parts[i + 1]);
                        args.Dissipate = true; break;
                    case "revert":
                        args.RevertType = byte.Parse(parts[i + 1]);
                        args.Revert = true; break;
                    case "door":
                        args.Door = true; break;
                    case "explode":
                        args.ExplodeNum = int.Parse(parts[i + 1]);
                        args.Explode = true; break;
                    case "rainbow":
                        args.RainbowNum = int.Parse(parts[i + 1]);
                        args.Rainbow = true; break;
                }
            }

            if (args.Wait) {
                if (args.Door && C.time < 2) {
                    Checktdoor(lvl, lvl.IntOffset(C.b, -1, 0, 0));
                    Checktdoor(lvl, lvl.IntOffset(C.b, 1, 0, 0));
                    Checktdoor(lvl, lvl.IntOffset(C.b, 0, -1, 0));
                    Checktdoor(lvl, lvl.IntOffset(C.b, 0, 1, 0));
                    Checktdoor(lvl, lvl.IntOffset(C.b, 0, 0, -1));
                    Checktdoor(lvl, lvl.IntOffset(C.b, 0, 0, 1));
                }

                if (C.time > args.WaitTime) {
                    int waitIndex = info.IndexOf("wait ");
                    C.data =
                        info.Substring(0, waitIndex) +
                        info.Substring(info.IndexOf(' ', waitIndex + 5) + 1);
                    DoOther(lvl, C, ref args);
                    return false;
                }
                C.time++;
                return true;
            }
            DoOther(lvl, C, ref args);
            return false;
        }
        
        static void DoOther(Level lvl, Check C, ref ExtraInfoArgs args) {
            Random rand = lvl.physRandom;			
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (args.Rainbow) {
                DoRainbow(lvl, C, rand, args.RainbowNum); return;
            }
            if (args.Revert) {
                lvl.AddUpdate(C.b, args.RevertType);
                C.data = "";
            }
            
            // Not setting drop = false can cause occasional leftover blocks, since C.extraInfo is emptied, so
            // drop can generate another block with no dissipate/explode information.
            if (args.Dissipate && rand.Next(1, 100) <= args.DissipateNum) {
            	if (!lvl.listUpdateExists.Get(x, y, z)) {
                    lvl.AddUpdate(C.b, Block.air);
                    C.data = "";
                    args.Drop = false;
                } else {
                    lvl.AddUpdate(C.b, lvl.blocks[C.b], false, C.data);
                }
            }
            
            if (args.Explode && rand.Next(1, 100) <= args.ExplodeNum) {
                lvl.MakeExplosion(x, y, z, 0);
                C.data = "";
                args.Drop = false;
            }
            
            if (args.Drop && rand.Next(1, 100) <= args.DropNum)
                DoDrop(lvl, C, rand, args.DropNum, x, y, z);
        }
        
        static void DoRainbow(Level lvl, Check C, Random rand, int rainbownum) {
            if (C.time < 4) {
                C.time++; return;
            }
            
            if (rainbownum > 2) {
                byte block = lvl.blocks[C.b];
                if (block < Block.red || block > Block.darkpink) {
                    lvl.AddUpdate(C.b, Block.red, true, C.data);
                } else {
                    byte next = block == Block.darkpink ? Block.red : (byte)(block + 1);
                    lvl.AddUpdate(C.b, next);
                }
            } else {
                lvl.AddUpdate(C.b, rand.Next(Block.red, Block.darkpink + 1));
            }
        }
        
        static void DoDrop(Level lvl, Check C, Random rand, int dropnum, ushort x, ushort y, ushort z) {
            int index = lvl.PosToInt(x, (ushort)(y - 1), z);
            if (index < 0) return;
            
            byte below = lvl.blocks[index];
            if (!(below == Block.air || below == Block.lava || below == Block.water))
                return;
            
            if (rand.Next(1, 100) < dropnum && lvl.AddUpdate(index, lvl.blocks[C.b], false, C.data)) {
                lvl.AddUpdate(C.b, Block.air);
                C.data = "";
            }
        }
        
        struct ExtraInfoArgs {
            public bool Wait, Drop, Dissipate, Revert, Door, Explode, Rainbow;
            public int WaitTime, DropNum, DissipateNum, ExplodeNum, RainbowNum;
            public byte RevertType;
        }
    }
}