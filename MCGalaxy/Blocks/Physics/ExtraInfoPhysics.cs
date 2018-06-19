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
using BlockID = System.UInt16;

namespace MCGalaxy.Blocks.Physics {
    
    public delegate bool ExtraInfoHandler(Level lvl, ref PhysInfo C);
    
    public static class ExtraInfoPhysics {
        
        public static bool DoDoorsOnly(Level lvl, ref PhysInfo C) {
            if (!C.Data.HasWait && C.Block == Block.Air)
                C.Data.ResetTypes();
            if (!C.Data.HasWait) return false;
            
            int waitTime = 0;
            if (C.Data.Type1 == PhysicsArgs.Wait) waitTime = C.Data.Value1;
            if (C.Data.Type2 == PhysicsArgs.Wait) waitTime = C.Data.Value2;
            
            if (C.Data.Data <= waitTime) { C.Data.Data++; return true; }
            if (C.Data.Type1 == PhysicsArgs.Wait) C.Data.Type1 = 0;
            if (C.Data.Type2 == PhysicsArgs.Wait) C.Data.Type2 = 0;
            
            return false;
        }
        
        public static bool DoNormal(Level lvl, ref PhysInfo C) {
            if (!C.Data.HasWait && C.Block == Block.Air)
                C.Data.ResetTypes();
            
            ExtraInfoArgs args = default(ExtraInfoArgs);
            ParseType(C.Data.Type1, ref args, C.Data.Value1);
            ParseType(C.Data.Type2, ref args, C.Data.Value2);
            args.ExtBlock = C.Data.ExtBlock;
            
            if (args.Wait) {
                if (C.Data.Data <= args.WaitTime) { C.Data.Data++; return true; }
                if (C.Data.Type1 == PhysicsArgs.Wait) C.Data.Type1 = 0;
                if (C.Data.Type2 == PhysicsArgs.Wait) C.Data.Type2 = 0;
            }
            DoOther(lvl, ref C, ref args);
            return false;
        }
        
        // cache to avoid allocation each time accessed
        internal static ExtraInfoHandler doorsHandler = DoDoorsOnly;
        internal static ExtraInfoHandler normalHandler = DoNormal;
        
        static void ParseType(byte type, ref ExtraInfoArgs args, byte value) {
            switch (type) {
                case PhysicsArgs.Wait:
                    args.Wait = true; args.WaitTime = value; break;
                case PhysicsArgs.Drop:
                    args.Drop = true; args.DropNum = value; break;
                case PhysicsArgs.Dissipate:
                    args.Dissipate = true; args.DissipateNum = value; break;
                case PhysicsArgs.Revert:
                    args.Revert = true; args.RevertType = value; break;
                case PhysicsArgs.Explode:
                    args.Explode = true; args.ExplodeNum = value; break;
                case PhysicsArgs.Rainbow:
                    args.Rainbow = true; args.RainbowNum = value; break;
            }
        }
        
        static void DoOther(Level lvl, ref PhysInfo C, ref ExtraInfoArgs args) {
            Random rand = lvl.physRandom;
            if (args.Rainbow) {
                if (C.Data.Data < 4) C.Data.Data++;
                else DoRainbow(lvl, ref C, rand, args.RainbowNum);
                return;
            }
            
            if (args.Revert) {
                PhysicsArgs revertArgs = default(PhysicsArgs);
                revertArgs.ExtBlock = args.ExtBlock;
                lvl.AddUpdate(C.Index, args.RevertType, revertArgs);
                
                C.Data.ResetTypes();
                C.Data.Data = PhysicsArgs.RemoveFromChecks;
            }
            
            ushort x = C.X, y = C.Y, z = C.Z;            
            // Not setting drop = false can cause occasional leftover blocks, since C.extraInfo is emptied, so
            // drop can generate another block with no dissipate/explode information.
            if (args.Dissipate && rand.Next(1, 100) <= args.DissipateNum) {
                if (!lvl.listUpdateExists.Get(x, y, z)) {
                    lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs));
                    C.Data.ResetTypes();
                    args.Drop = false;
                } else {
                    lvl.AddUpdate(C.Index, C.Block, C.Data);
                }
            }
            
            if (args.Explode && rand.Next(1, 100) <= args.ExplodeNum) {
                lvl.MakeExplosion(x, y, z, 0);
                C.Data.ResetTypes();
                args.Drop = false;
            }
            
            if (args.Drop && rand.Next(1, 100) <= args.DropNum)
                DoDrop(lvl, ref C, rand, args.DropNum, x, y, z);
        }
        
        static void DoRainbow(Level lvl, ref PhysInfo C, Random rand, int rainbownum) {
            if (rainbownum > 2) {
                BlockID block = C.Block;
                if (block < Block.Red || block > Block.Pink) {
                    lvl.AddUpdate(C.Index, Block.Red, C.Data);
                } else {
                    BlockID next = block == Block.Pink ? Block.Red : (BlockID)(block + 1);
                    lvl.AddUpdate(C.Index, next, default(PhysicsArgs));
                }
            } else {
                lvl.AddUpdate(C.Index, (BlockID)rand.Next(Block.Red, Block.Pink + 1), default(PhysicsArgs));
            }
        }
        
        static void DoDrop(Level lvl, ref PhysInfo C, Random rand, int dropnum, ushort x, ushort y, ushort z) {
            int index;
            BlockID below = lvl.GetBlock(x, (ushort)(y - 1), z, out index);
            if (!(below == Block.Air || below == Block.Lava || below == Block.Water)) return;
            
            if (rand.Next(1, 100) < dropnum && lvl.AddUpdate(index, C.Block, C.Data)) {
                lvl.AddUpdate(C.Index, Block.Air, default(PhysicsArgs));
                C.Data.ResetTypes();
            }
        }
        
        struct ExtraInfoArgs {
            public bool Wait, Drop, Dissipate, Revert, Explode, Rainbow;
            public int WaitTime, DropNum, DissipateNum, ExplodeNum, RainbowNum;
            public byte RevertType, ExtBlock;
        }
    }
}