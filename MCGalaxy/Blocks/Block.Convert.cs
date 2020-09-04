/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using BlockRaw = System.Byte;

namespace MCGalaxy {
    public static partial class Block {
        
        internal static string[] coreNames = new string[Block.Count];
        public static bool Undefined(BlockID block) { return IsPhysicsType(block) && coreNames[block].CaselessEq("unknown"); }
        
        public static bool ExistsGlobal(BlockID b) { return ExistsFor(Player.Console, b); }
        
        public static bool ExistsFor(Player p, BlockID b) {
            if (b < Block.Count) return !Undefined(b);
            
            if (!p.IsSuper) return p.level.GetBlockDef(b) != null;
            return BlockDefinition.GlobalDefs[b] != null;
        }
        
        public static string GetName(Player p, BlockID block) {
            if (IsPhysicsType(block)) return coreNames[block];
            
            BlockDefinition def;
            if (!p.IsSuper) {
                def = p.level.GetBlockDef(block);
            } else {
                def = BlockDefinition.GlobalDefs[block];
            }
            if (def != null) return def.Name.Replace(" ", "");
            
            return block < Block.Extended ? coreNames[block] : ToRaw(block).ToString();
        }
        
        public static BlockID Parse(Player p, string input) {
            BlockDefinition[] defs = p.IsSuper ? BlockDefinition.GlobalDefs : p.level.CustomBlockDefs;
            BlockID block;
            // raw ID is treated specially, before names
            if (BlockID.TryParse(input, out block)) {
                if (block < Block.CpeCount || (block <= Block.MaxRaw && defs[FromRaw(block)] != null)) {
                    return FromRaw(block);
                }
            }
            
            block = GetBlockByName(input, defs);
            if (block != Block.Invalid) return block;
            
            byte coreID;
            bool success = Aliases.TryGetValue(input.ToLower(), out coreID);
            return success ? coreID : Invalid;
        }
        
        static BlockID GetBlockByName(string msg, BlockDefinition[] defs) {
            for (int i = 1; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def == null) continue;
                if (def.Name.Replace(" ", "").CaselessEq(msg)) return def.GetBlock();
            }
            return Block.Invalid;
        }
        
        public static byte ConvertCPE(byte block) {
            switch (block) {
                case CobblestoneSlab: return Slab;
                case Rope: return Mushroom;
                case Sandstone: return Sand;
                case Snow: return Air;
                case Fire: return Lava;
                case LightPink: return Pink;
                case ForestGreen: return Green;
                case Brown: return Dirt;
                case DeepBlue: return Blue;
                case Turquoise: return Cyan;
                case Ice: return Glass;
                case CeramicTile: return Iron;
                case MagmaBlock: return Obsidian;
                case Pillar: return White;
                case Crate: return Wood;
                case StoneBrick: return Stone;
                default: return block;
            }
        }
        
        public static BlockID Convert(BlockID block) {
            switch (block) {
                case FlagBase: return Mushroom;
                case Op_Glass: return Glass;
                case Op_Obsidian: return Obsidian;
                case Op_Brick: return Brick;
                case Op_Stone: return Stone;
                case Op_Cobblestone: return Cobblestone;
                case Op_Air: return Air; //Must be cuboided / replaced
                case Op_Water: return StillWater;
                case Op_Lava: return StillLava;

                case 108: return Cobblestone;
                case LavaSponge: return Sponge;

                case FloatWood: return Wood;
                case FastLava: return Lava;
                case 71:
                case 72:
                    return White;
                case Door_Log: return Log;
                case Door_Obsidian: return Obsidian;
                case Door_Glass: return Glass;
                case Door_Stone: return Stone;
                case Door_Leaves: return Leaves;
                case Door_Sand: return Sand;
                case Door_Wood: return Wood;
                case Door_Green: return Green;
                case Door_TNT: return TNT;
                case Door_Slab: return Slab;
                case Door_Iron: return Iron;
                case Door_Dirt: return Dirt;
                case Door_Grass: return Grass;
                case Door_Blue: return Blue;
                case Door_Bookshelf: return Bookshelf;
                case Door_Gold: return Gold;
                case Door_Cobblestone: return Cobblestone;
                case Door_Red: return Red;

                case Door_Orange: return Orange;
                case Door_Yellow: return Yellow;
                case Door_Lime: return Lime;
                case Door_Teal: return Teal;
                case Door_Aqua: return Aqua;
                case Door_Cyan: return Cyan;
                case Door_Indigo: return Indigo;
                case Door_Purple: return Violet;
                case Door_Magenta: return Magenta;
                case Door_Pink: return Pink;
                case Door_Black: return Black;
                case Door_Gray: return Gray;
                case Door_White: return White;

                case tDoor_Log: return Log;
                case tDoor_Obsidian: return Obsidian;
                case tDoor_Glass: return Glass;
                case tDoor_Stone: return Stone;
                case tDoor_Leaves: return Leaves;
                case tDoor_Sand: return Sand;
                case tDoor_Wood: return Wood;
                case tDoor_Green: return Green;
                case tDoor_TNT: return TNT;
                case tDoor_Slab: return Slab;
                case tDoor_Air: return Air;
                case tDoor_Water: return StillWater;
                case tDoor_Lava: return StillLava;

                case oDoor_Log: return Log;
                case oDoor_Obsidian: return Obsidian;
                case oDoor_Glass: return Glass;
                case oDoor_Stone: return Stone;
                case oDoor_Leaves: return Leaves;
                case oDoor_Sand: return Sand;
                case oDoor_Wood: return Wood;
                case oDoor_Green: return Green;
                case oDoor_TNT: return TNT;
                case oDoor_Slab: return Slab;
                case oDoor_Lava: return StillLava;
                case oDoor_Water: return StillWater;

                case MB_White: return White;
                case MB_Black: return Black;
                case MB_Air: return Air;
                case MB_Water: return StillWater;
                case MB_Lava: return StillLava;

                case WaterDown: return Water;
                case LavaDown: return Lava;
                case WaterFaucet: return Aqua;
                case LavaFaucet: return Orange;

                case FiniteWater: return Water;
                case FiniteLava: return Lava;
                case FiniteFaucet: return Cyan;

                case Portal_Air: return Air;
                case Portal_Water: return StillWater;
                case Portal_Lava: return StillLava;

                case Door_Air: return Air;
                case Door_AirActivatable: return Air;
                case Door_Water: return StillWater;
                case Door_Lava: return StillLava;

                case Portal_Blue: return Cyan;
                case Portal_Orange: return Orange;

                case C4: return TNT;
                case C4Detonator: return Red;
                case TNT_Small: return TNT;
                case TNT_Big: return TNT;
                case TNT_Explosion: return Lava;

                case LavaFire: return Lava;
                case TNT_Nuke: return TNT;

                case RocketStart: return Glass;
                case RocketHead: return Gold;
                case Fireworks: return Iron;

                case Deadly_Water: return StillWater;
                case Deadly_Lava: return StillLava;
                case Deadly_Air: return Air;
                case Deadly_ActiveWater: return Water;
                case Deadly_ActiveLava: return Lava;
                case Deadly_FastLava: return Lava;

                case Magma: return Lava;
                case Geyser: return Water;
                case Checkpoint: return Air;

                case Air_Flood:
                case Door_Log_air:
                case Air_FloodLayer:
                case Air_FloodDown:
                case Air_FloodUp:
                case 205:
                case 206:
                case 207:
                case 208:
                case 209:
                case 210:
                case 213:
                case 214:
                case 215:
                case 216:
                case Door_Air_air:
                case 225:
                case 254:
                case 81:
                case 226:
                case 227:
                case 228:
                case 229:
                case 84:
                case 66:
                case 67:
                case 68:
                case 69:
                    return Air;
                case Door_Green_air: return Red;
                case Door_TNT_air: return Lava;

                case oDoor_Log_air:
                case oDoor_Obsidian_air:
                case oDoor_Glass_air:
                case oDoor_Stone_air:
                case oDoor_Leaves_air:
                case oDoor_Sand_air:
                case oDoor_Wood_air:
                case oDoor_Slab_air:
                case oDoor_Lava_air:
                case oDoor_Water_air:
                    return Air;
                case oDoor_Green_air: return Red;
                case oDoor_TNT_air: return StillLava;

                case Train: return Aqua;

                case Snake: return Black;
                case SnakeTail: return CoalOre;

                case Creeper: return TNT;
                case ZombieBody: return MossyRocks;
                case ZombieHead: return Lime;

                case Bird_White: return White;
                case Bird_Black: return Black;
                case Bird_Lava: return Lava;
                case Bird_Red: return Red;
                case Bird_Water: return Water;
                case Bird_Blue: return Blue;
                case Bird_Killer: return Lava;

                case Fish_Betta: return Blue;
                case Fish_Gold: return Gold;
                case Fish_Salmon: return Red;
                case Fish_Shark: return Gray;
                case Fish_Sponge: return Sponge;
                case Fish_LavaShark: return Obsidian;
            }
            return block;
        }
    }
}
