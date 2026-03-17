/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Blocks;
using BlockID = System.UInt16;

namespace MCGalaxy 
{
    public static partial class Block 
    {        
        public static BlockProps[] Props = new BlockProps[Block.SUPPORTED_COUNT];
        
        internal static BlockProps MakeDefaultProps(BlockID b) {
            BlockProps props = BlockProps.MakeEmpty();
            if ((b >= Op_Glass && b <= Op_Lava) || b == Invalid || b == RocketStart || b == Bedrock) {
                props.OPBlock = true;
            }
            
            if ((b >= tDoor_Log && b <= tDoor_Green) || (b >= tDoor_TNT && b <= tDoor_Lava)) {
                props.IsTDoor = true;
            }
            if (b >= MB_White && b <= MB_Lava) {
                props.IsMessageBlock = true;
            }
            if (b == Portal_Blue || b == Portal_Orange || (b >= Portal_Air && b <= Portal_Lava)) {
                props.IsPortal = true;
            }
            
            // oDoor blocks
            if (b >= oDoor_Log && b <= oDoor_Wood) {
                props.oDoorBlock = (BlockID)(oDoor_Log_air + (b - oDoor_Log));
            }
            if (b >= oDoor_Green && b <= oDoor_Water) {
                props.oDoorBlock = (BlockID)(oDoor_Green_air + (b - oDoor_Green));
            }
            if (b >= oDoor_Log_air && b <= oDoor_Wood_air) {
                props.oDoorBlock = (BlockID)(oDoor_Log + (b - oDoor_Log_air));
            }
            if (b >= oDoor_Green_air && b <= oDoor_Water_air) {
                props.oDoorBlock = (BlockID)(oDoor_Green + (b - oDoor_Green_air));
            }
            
            // Water/Lava kills
            props.LavaKills = b == Wood || b == Log 
                || b == Sponge || b == Bookshelf || b == Leaves || b == Crate;
            
            if ((b >= Red && b <= White) || (b >= LightPink && b <= Turquoise)) {
                props.LavaKills = true;
            }
            if (b == Air || b == Sapling || (b >= Dandelion && b <= RedMushroom)) {
                props.LavaKills  = true;
                props.WaterKills = true;
            }
            
            props.IsDoor   = IsDoor(b);
            props.AnimalAI = GetAI(b);
            props.IsRails  = b == Red || b == Op_Air;
            
            props.Drownable = b >= Water && b <= StillLava;
            if (b == Water || b == StillWater) props.DeathMessage = "@p &S&cdrowned.";
            if (b == Lava  || b == StillLava)  props.DeathMessage = "@p &Sburnt to a &ccrisp.";
            if (b == Air) props.DeathMessage = "@p &Shit the floor &chard.";
            
            string deathMsg = GetDeathMessage(b);
            if (deathMsg != null) {
                props.DeathMessage = deathMsg;
                props.KillerBlock  = true;
            }
            
            // Block specific properties
            if (b == Block.Slab)            props.StackBlock = DoubleSlab;
            if (b == Block.CobblestoneSlab) props.StackBlock = Cobblestone;
            if (b == Block.Dirt)            props.GrassBlock = Block.Grass;
            if (b == Block.Grass)           props.DirtBlock  = Block.Dirt;
            return props;
        }
        
        static bool IsDoor(BlockID b) {
            if (b >= Door_Obsidian && b <= Door_Slab)  return true;
            if (b >= Door_Iron && b <= Door_Bookshelf) return true;
            if (b >= Door_Orange && b <= Door_White)   return true;
            if (b >= Door_Air && b <= Door_Lava)       return true;
            return b == Door_Cobblestone || b == Door_Red || b == Door_Log || b == Door_Gold;
        }
        
        static AnimalAI GetAI(BlockID b) {
            if (b == Bird_Black || b == Bird_White || b == Bird_Lava || b == Bird_Water) return AnimalAI.Fly;
            if (b == Bird_Red   || b == Bird_Blue  || b == Bird_Killer) return AnimalAI.KillerAir;

            if (b == Fish_Betta || b == Fish_Shark) return AnimalAI.KillerWater;
            if (b == Fish_LavaShark)                return AnimalAI.KillerLava;           
            if (b == Fish_Gold  || b == Fish_Salmon || b == Fish_Sponge) return AnimalAI.FleeWater;
            
            return AnimalAI.None;
        }
        
        static string GetDeathMessage(BlockID b) {
            if (b == TNT_Explosion) return "@p &S&cblew into pieces.";
            if (b == Deadly_Air) return "@p &Swalked into &cnerve gas and suffocated.";
            
            if (b == Deadly_Water || b == Deadly_ActiveWater)
                return "@p &Sstepped in &dcold water and froze.";
            if (b == Deadly_Lava  || b == Deadly_ActiveLava || b == Deadly_FastLava)
                return "@p &Sstood in &cmagma and melted.";
            
            if (b == Magma)  return "@p &Swas hit by &cflowing magma and melted.";
            if (b == Geyser) return "@p &Swas hit by &cboiling water and melted.";
            if (b == Bird_Killer) return "@p &Swas hit by a &cphoenix and burnt.";
            if (b == Train)       return "@p &Swas hit by a &ctrain.";
            if (b == Fish_Shark)  return "@p &Swas eaten by a &cshark.";
            if (b == LavaFire)    return "@p &Sburnt to a &ccrisp.";
            if (b == RocketHead)  return "@p &Swas &cin a fiery explosion.";
            if (b == ZombieBody)  return "@p &Sdied due to lack of &5brain.";
            if (b == Creeper)     return "@p &Swas killed &cb-SSSSSSSSSSSSSS";
            if (b == Fish_LavaShark) return "@p &Swas eaten by a ... LAVA SHARK?!";
            if (b == Snake)       return "@p &Swas bit by a deadly snake.";
            
            return null;
        }
    }
}
