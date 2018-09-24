/*
    Copyright 2015 MCGalaxy
        
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
using System.Collections.Generic;
using MCGalaxy.Blocks;
using BlockID = System.UInt16;

namespace MCGalaxy {
    public static partial class Block {
        
        public static BlockProps[] Props = new BlockProps[Block.ExtendedCount];
        public static Dictionary<string, byte> Aliases = new Dictionary<string, byte>();
        
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
            
            if ((b >= Red && b <= White) || (b >= LightPink && b <= turquoise)) {
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
            if (props.Drownable) props.DeathMessage = "@p %S&cdrowned.";
            if (b == Air) props.DeathMessage = "@p %Shit the floor &chard.";
            
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
            if (b == TNT_Explosion) return "@p %S&cblew into pieces.";
            if (b == Deadly_Air) return "@p %Swalked into &cnerve gas and suffocated.";
            
            if (b == Deadly_Water || b == Deadly_ActiveWater)
                return "@p %Sstepped in &dcold water and froze.";
            if (b == Deadly_Lava  || b == Deadly_ActiveLava || b == Deadly_FastLava)
                return "@p %Sstood in &cmagma and melted.";
            
            if (b == Magma)  return "@p %Swas hit by &cflowing magma and melted.";
            if (b == Geyser) return "@p %Swas hit by &cboiling water and melted.";
            if (b == Bird_Killer) return "@p %Swas hit by a &cphoenix and burnt.";
            if (b == Train)       return "@p %Swas hit by a &ctrain.";
            if (b == Fish_Shark)  return "@p %Swas eaten by a &cshark.";
            if (b == LavaFire)    return "@p %Sburnt to a &ccrisp.";
            if (b == RocketHead)  return "@p %Swas &cin a fiery explosion.";
            if (b == ZombieBody)  return "@p %Sdied due to lack of &5brain.";
            if (b == Creeper)     return "@p %Swas killed &cb-SSSSSSSSSSSSSS";
            if (b == Fish_LavaShark) return "@p %Swas eaten by a ... LAVA SHARK?!";
            if (b == Snake)       return "@p %Swas bit by a deadly snake.";
            
            return null;
        }
        
        internal static void SetDefaultNames() {
            string[] names = new string[] { "Air", "Stone", "Grass", "Dirt", "Cobblestone",
                "Wood", "Sapling", "Bedrock", "Active_Water", "Water", "Active_Lava", "Lava",
                "Sand", "Gravel", "Gold_Ore", "Iron_Ore", "Coal", "Log", "Leaves", "Sponge",
                "Glass", "Red", "Orange", "Yellow", "Lime", "Green", "Teal", "Aqua", "Cyan",
                "Blue", "Indigo", "Violet", "Magenta", "Pink", "Black", "Gray", "White",
                "Dandelion", "Rose", "Brown_Shroom", "Red_Shroom", "Gold", "Iron",
                "DoubleSlab", "Slab", "Brick", "TNT", "BookShelf", "MossyRocks",
                "Obsidian", "CobblestoneSlab", "Rope", "SandStone", "Snow", "Fire", "LightPink",
                "ForestGreen", "Brown", "DeepBlue", "Turquoise", "Ice", "CeramicTile", "MagmaBlock",
                "Pillar", "Crate", "StoneBrick", null, null,
                null, null, "FlagBase", null, null,
                "Fast_Hot_Lava", "C4", "C4_Det", null, null, null, null,
                "Door_Cobblestone", null, null, "Door_Red", null,
                "Door_Orange", "Door_Yellow", "Door_LightGreen", null, "Door_AquaGreen",
                "Door_Cyan", "Door_LightBlue", "Door_Purple", "Door_LightPurple", "Door_Pink",
                "Door_DarkPink", "Door_DarkGrey", "Door_LightGrey", "Door_White", null,
                "Op_Glass", "Opsidian", "Op_Brick", "Op_Stone", "Op_Cobblestone", "Op_Air",
                "Op_Water", "Op_Lava", null, "Lava_Sponge", "Wood_Float", "Door",
                "Lava_Fast", "Door_Obsidian", "Door_Glass", "Door_Stone", "Door_Leaves", "Door_Sand",
                "Door_Wood", "Door_Green", "Door_TNT", "Door_Stair", "tDoor", "tDoor_Obsidian",
                "tDoor_Glass", "tDoor_Stone", "tDoor_Leaves", "tDoor_Sand", "tDoor_Wood",
                "tDoor_Green", "White_Message", "Black_Message", "Air_Message", "Water_Message",
                "Lava_Message", "tDoor_TNT", "tDoor_Stair", "tDoor_Air", "tDoor_Water", "tDoor_lava",
                "Waterfall", "Lavafall", null, "Water_Faucet", "Lava_Faucet", "Finite_Water",
                "Finite_Lava", "Finite_Faucet", "oDoor", "oDoor_Obsidian", "oDoor_Glass",
                "oDoor_Stone", "oDoor_Leaves", "oDoor_Sand", "oDoor_Wood", "oDoor_Green",
                "oDoor_TNT", "oDoor_Stair", "oDoor_Lava", "oDoor_Water", "Air_Portal", "Water_Portal",
                "Lava_Portal", "Custom_Block", "Air_Door", "Air_Switch", "Door_Water", "Door_Lava",
                "oDoor_Air", "oDoor_Obsidian_Air", "oDoor_Glass_Air", "oDoor_Stone_Air",
                "oDoor_Leaves_Air", "oDoor_Sand_Air", "oDoor_Wood_Air", "Blue_Portal", "Orange_Portal",
                "oDoor_Red", "oDoor_TNT_Air", "oDoor_Stair_Air", "oDoor_Lava_Air", "oDoor_Water_Air",
                "Small_TNT", "Big_TNT", "TNT_Explosion", "Lava_Fire", "Nuke_TNT", "RocketStart",
                "RocketHead", "Firework", "Hot_Lava", "Cold_Water", "Nerve_Gas", "Active_Cold_Water",
                "Active_Hot_Lava", "Magma", "Geyser", "Checkpoint", null, null, "Air_Flood",
                "Door_Air", "Air_Flood_Layer", "Air_Flood_Down", "Air_Flood_Up", null,
                null, null, null, null, null, "Door8_Air",
                "Door9_Air", null, null, null, null, null,
                null, null, "Door_Iron", "Door_Dirt", "Door_Grass", "Door_Blue", "Door_Book",
                null, null, null, null, null,
                "Train", "Creeper", "Zombie", "Zombie_Head", null, "Dove", "Pidgeon", "Duck",
                "Phoenix", "Red_Robin", "Blue_Bird", null, "Killer_Phoenix", null, null,
                "GoldFish", "Sea_Sponge", "Shark", "Salmon", "Betta_Fish", "Lava_Shark", "Snake",
                "Snake_Tail", "Door_Gold", null, null };
            
            Aliases.Clear();
            SetDefaultAliases();
            for (int i = 0; i < names.Length; i++) {
                string name = names[i];
                if (name == null) name = "unknown";
                if (i > 0 && i < Block.CpeCount) {
                    BlockDefinition def = BlockDefinition.GlobalDefs[i];
                    if (def != null) name = def.Name;
                }
                coreNames[i] = name;
                
                name = name.ToLower();
                if (name != "unknown")
                    Aliases[name] = (byte)i;
                if (name.IndexOf('_') >= 0)
                    Aliases[name.Replace("_", "")] = (byte)i;
            }
        }
        
        static void SetDefaultAliases() {
            Dictionary<string, byte> aliases = Aliases;
            // Add old MCGalaxy names
            aliases["purple"] = Indigo; aliases["blueviolet"] = Blue;
            aliases["adminium"] = Bedrock; aliases["bookcase"] = Bookshelf;
            aliases["plant"] = Sapling; aliases["mossy_cobblestone"] = MossyRocks;
            aliases["springgreen"] = Teal; aliases["greenyellow"] = Lime;
            aliases["red_flower"] = Rose; aliases["yellow_flower"] = Dandelion;
            aliases["stair"] = Slab; aliases["double_stair"] = DoubleSlab;
            // Add other aliases
            aliases["planks"] = Wood; aliases["tree"] = Log;
            aliases["stairs"] = Slab; aliases["slab"] = Slab;
            aliases["doubleslab"] = DoubleSlab; aliases["slabfull"] = DoubleSlab;
            aliases["solid"] = Bedrock; aliases["admintite"] = Bedrock;
            aliases["blackrock"] = Bedrock; aliases["activewater"] = Water;
            aliases["activelava"] = Lava; aliases["fhl"] = Deadly_FastLava;
            aliases["water_door"] = Door_Water; aliases["lava_door"] = Door_Lava;
            aliases["acw"] = Deadly_ActiveWater; aliases["ahl"] = Deadly_ActiveLava;
            
            aliases["door_tree"] = Door_Log; aliases["door2"] = Door_Obsidian;
            aliases["door3"] = Door_Glass; aliases["door4"] = Door_Stone;
            aliases["door5"] = Door_Leaves; aliases["door6"] = Door_Sand;
            aliases["door7"] = Door_Wood; aliases["door8"] = Door_Green;
            aliases["door9"] = Door_TNT; aliases["door10"] = Door_Slab;
            aliases["door11"] = Door_Iron; aliases["door12"] = Door_Dirt;
            aliases["door13"] = Door_Grass; aliases["door14"] = Door_Blue;
            aliases["door15"] = Door_Bookshelf; aliases["door16"] = Door_Gold;
            aliases["door17"] = Door_Cobblestone; aliases["door18"] = Door_Red;
            
            aliases["tdoor_tree"] = tDoor_Log; aliases["tdoor2"] = tDoor_Obsidian;
            aliases["tdoor3"] = tDoor_Glass; aliases["tdoor4"] = tDoor_Stone;
            aliases["tdoor5"] = tDoor_Leaves; aliases["tdoor6"] = tDoor_Sand;
            aliases["tdoor7"] = tDoor_Wood; aliases["tdoor8"] = tDoor_Green;
            aliases["tdoor9"] = tDoor_TNT; aliases["tdoor10"] = tDoor_Slab;
            aliases["tair_switch"] = tDoor_Air; aliases["tdoor11"] = tDoor_Air;
            aliases["tdoor12"] = tDoor_Water; aliases["tdoor13"] = tDoor_Lava;
            
            aliases["odoor_tree"] = oDoor_Log; aliases["odoor2"] = oDoor_Obsidian;
            aliases["odoor3"] = oDoor_Glass; aliases["odoor4"] = oDoor_Stone;
            aliases["odoor5"] = oDoor_Leaves; aliases["odoor6"] = oDoor_Sand;
            aliases["odoor7"] = oDoor_Wood; aliases["odoor8"] = oDoor_Green;
            aliases["odoor9"] = oDoor_TNT; aliases["odoor10"] = oDoor_Slab;
            aliases["odoor11"] = oDoor_Lava; aliases["odoor12"] = oDoor_Water;
            
            aliases["steps"] = Slab; aliases["double_steps"] = DoubleSlab;
            aliases["step"] = Slab; aliases["double_step"] = DoubleSlab;
            aliases["grey"] = Gray; aliases["door_darkgray"] = Door_Black;
            aliases["door_lightgray"] = Door_Gray;
        }
    }
}
