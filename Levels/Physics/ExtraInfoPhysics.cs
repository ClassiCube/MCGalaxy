/*
    Copyright 2015 MCGalaxy
    Original level physics copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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
        
        public static bool DoComplex(Level lvl, Check C, Random rand) {
		    string info = C.data as string;
		    if (info == null) return true;			
            if (!info.Contains("wait") && lvl.blocks[C.b] == Block.air)
                C.data = "";

            bool wait = false, drop = false, dissipate = false, revert = false, door = false;
            int waitTime = 0, dropnum = 0, dissipatenum = 0; byte reverttype = 0;
            string[] parts = info.Split(' ');
            
            for (int i = 0; i < parts.Length; i++) {
                if (i % 2 != 0) continue;
                
                switch (parts[i]) {
                    case "wait":
                        waitTime = int.Parse(parts[i + 1]);
                        wait = true; break;
                    case "drop":
                        dropnum = int.Parse(parts[i + 1]);
                        drop = true; break;
                    case "dissipate":
                        dissipatenum = int.Parse(parts[i + 1]);
                        dissipate = true; break;
                    case "revert":
                        reverttype = byte.Parse(parts[i + 1]);
                        revert = true; break;
                    case "door":
                        door = true; break;
                }
            }

            if (wait) {
                if (door && C.time < 2) {
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
                    DoOther(lvl, C, rand, revert, dissipate, drop, reverttype, dissipatenum, dropnum);
                    return false;
                }
                C.time++;
                return true;
            }
            DoOther(lvl, C, rand, revert, dissipate, drop, reverttype, dissipatenum, dropnum);
            return false;
        }
        
        static void DoOther(Level lvl, Check C, Random rand, bool revert, bool dissipate,
                            bool drop, byte reverttype, int dissipatenum, int dropnum) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            if (revert) {
                lvl.AddUpdate(C.b, reverttype);
                C.data = "";
            }
            
            // Not setting drop = false can cause occasional leftover blocks, since C.extraInfo is emptied, so
            // drop can generate another block with no dissipate/explode information.
            if (dissipate && rand.Next(1, 100) <= dissipatenum) {
                if (!lvl.ListUpdate.Exists(Update => Update.b == C.b)) {
                    lvl.AddUpdate(C.b, Block.air);
                    C.data = "";
                    drop = false;
                } else {
                    lvl.AddUpdate(C.b, lvl.blocks[C.b], false, C.data);
                }
            }
            
            if (drop && rand.Next(1, 100) <= dropnum)
                DoDrop(lvl, C, rand, dropnum, x, y, z);
        }
        
        static void DoDrop(Level lvl, Check C, Random rand, int dropnum, ushort x, ushort y, ushort z) {
            int index = lvl.PosToInt(x, (ushort)(y - 1), z);
            if (index < 0)
                return;
            
            byte below = lvl.blocks[index];
            if (!(below == Block.air || below == Block.lava || below == Block.water))
                return;
            
            if (rand.Next(1, 100) < dropnum && lvl.AddUpdate(index, lvl.blocks[C.b], false, C.data)) {
                lvl.AddUpdate(C.b, Block.air);
                C.data = "";
            }
        }
    }
}