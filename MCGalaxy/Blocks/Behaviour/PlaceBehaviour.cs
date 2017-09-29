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
using MCGalaxy.Blocks.Physics;
using System;

namespace MCGalaxy.Blocks {
    
    internal static class PlaceBehaviour {

        internal static void Grass(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            DirtGrass(p, (ExtBlock)Block.Grass, x, y, z);
        }
        
        internal static void Dirt(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            DirtGrass(p, (ExtBlock)Block.Dirt, x, y, z);
        }
        
        static void DirtGrass(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            Level lvl = p.level;
            if (!lvl.Config.GrassGrow || !(lvl.physics == 0 || lvl.physics == 5)) {
                p.ChangeBlock(x, y, z, block); return;
            }
            if (p.ModeBlock.BlockID == Block.Dirt || p.ModeBlock.BlockID == Block.Grass) {
                p.ChangeBlock(x, y, z, block); return;
            }
            
            ExtBlock above = lvl.GetBlock(x, (ushort)(y + 1), z);
            block.BlockID = (above.BlockID == Block.Invalid || lvl.LightPasses(above)) ? Block.Grass : Block.Dirt;
            p.ChangeBlock(x, y, z, block);
        }
        
        internal static HandlePlace Stack(ExtBlock block) {
            return (p, b, x, y, z) => Stack(p, block, x, y, z);
        }
        static void Stack(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.GetBlock(x, (ushort)(y - 1), z) != block) {
                p.ChangeBlock(x, y, z, block); return;
            }
            
            p.SendBlockchange(x, y, z, ExtBlock.Air); // send the air block back only to the user
            byte stack = p.level.Props[block.Index].StackId;
            p.ChangeBlock(x, (ushort)(y - 1), z, ExtBlock.FromRaw(stack));
        }        
        
        internal static void C4(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) {
                p.RevertBlock(x, y, z); return;
            }
            
            // Use red wool to detonate c4
            ExtBlock held = p.BlockBindings[p.RawHeldBlock.RawID];
            if (held.BlockID == Block.Red) {
                Player.Message(p, "Placed detonator block, delete it to detonate.");
                C4Det(p, (ExtBlock)Block.C4Detonator, x, y, z); return;
            }
            
            if (p.c4circuitNumber == -1) {
                sbyte num = C4Physics.NextCircuit(p.level);
                p.level.C4list.Add(new C4Data(num));
                p.c4circuitNumber = num;
                
                string detonatorName = p.level.BlockName((ExtBlock)Block.Red);
                Player.Message(p, "Place more blocks for more c4, then place a &c{0} %Sblock for the detonator.", 
                               detonatorName);
            }
            
            C4Data c4 = C4Physics.Find(p.level, p.c4circuitNumber);
            if (c4 != null) c4.list.Add(p.level.PosToInt(x, y, z));
            p.ChangeBlock(x, y, z, (ExtBlock)Block.C4);
        }
        
        internal static void C4Det(Player p, ExtBlock block, ushort x, ushort y, ushort z) {
            if (p.level.physics == 0 || p.level.physics == 5) {
                p.c4circuitNumber = -1;
                p.RevertBlock(x, y, z); return;
            }
            
            C4Data c4 = C4Physics.Find(p.level, p.c4circuitNumber);
            if (c4 != null) c4.detIndex = p.level.PosToInt(x, y, z);
            p.c4circuitNumber = -1;
            p.ChangeBlock(x, y, z, (ExtBlock)Block.C4Detonator);
        }
    }
}
