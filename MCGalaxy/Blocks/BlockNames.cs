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
    public static class BlockNames
    {
        internal static string[] coreNames = new string[Block.CORE_COUNT];
        public static Dictionary<string, byte> Aliases = new Dictionary<string, byte>();
        
        public static void UpdateCore() {
            Dictionary<string, byte> aliases = CreateDefaultAliases();
            int start = 0;
            
            for (int b = 0; b < Block.CORE_COUNT; b++)
            {
                int end = DEFAULT_NAMES.IndexOf('@', start);
                string name = start == end ? "unknown" : DEFAULT_NAMES.Substring(start, end - start);
                start = end + 1;
                
                if (b > 0 && b < Block.CPE_COUNT) {
                    BlockDefinition def = BlockDefinition.GlobalDefs[b];
                    if (def != null) name = def.Name;
                }
                coreNames[b] = name;
                
                name = name.ToLower();
                if (name != "unknown")
                    aliases[name] = (byte)b;
                if (name.IndexOf('_') >= 0)
                    aliases[name.Replace("_", "")] = (byte)b;
            }
            Aliases = aliases;
        }
        
        static Dictionary<string, byte> CreateDefaultAliases() {
            return new Dictionary<string, byte>() {
                // Add old MCGalaxy names
                { "purple",        Block.Indigo },
                { "blueviolet",    Block.Blue },
                { "adminium",      Block.Bedrock },
                { "bookcase",      Block.Bookshelf },
                { "plant",         Block.Sapling },
                { "mossy_cobblestone", Block.MossyRocks },
                { "springgreen",   Block.Teal },
                { "greenyellow",   Block.Lime },
                { "red_flower",    Block.Rose },
                { "yellow_flower", Block.Dandelion },
                { "stair",         Block.Slab },
                { "double_stair",  Block.DoubleSlab },
                
                // Add other aliases
                { "planks",      Block.Wood },
                { "tree",        Block.Log },
                { "stairs",      Block.Slab },
                { "slab",        Block.Slab },
                { "doubleslab",  Block.DoubleSlab },
                { "slabfull",    Block.DoubleSlab },
                { "solid",       Block.Bedrock },
                { "admintite",   Block.Bedrock },
                { "blackrock",   Block.Bedrock },
                { "activewater", Block.Water },
                { "activelava",  Block.Lava },
                { "fhl",         Block.Deadly_FastLava },
                { "water_door",  Block.Door_Water },
                { "lava_door",   Block.Door_Lava },
                { "acw",         Block.Deadly_ActiveWater },
                { "ahl",         Block.Deadly_ActiveLava },
                
                { "door_tree", Block.Door_Log },
                { "door2",     Block.Door_Obsidian },
                { "door3",     Block.Door_Glass },
                { "door4",     Block.Door_Stone },
                { "door5",     Block.Door_Leaves },
                { "door6",     Block.Door_Sand },
                { "door7",     Block.Door_Wood },
                { "door8",     Block.Door_Green },
                { "door9",     Block.Door_TNT },
                { "door10",    Block.Door_Slab },
                { "door11",    Block.Door_Iron },
                { "door12",    Block.Door_Dirt },
                { "door13",    Block.Door_Grass },
                { "door14",    Block.Door_Blue },
                { "door15",    Block.Door_Bookshelf },
                { "door16",    Block.Door_Gold },
                { "door17",    Block.Door_Cobblestone },
                { "door18",    Block.Door_Red },
                
                { "tdoor_tree",  Block.tDoor_Log },
                { "tdoor2",      Block.tDoor_Obsidian },
                { "tdoor3",      Block.tDoor_Glass },
                { "tdoor4",      Block.tDoor_Stone },
                { "tdoor5",      Block.tDoor_Leaves },
                { "tdoor6",      Block.tDoor_Sand },
                { "tdoor7",      Block.tDoor_Wood },
                { "tdoor8",      Block.tDoor_Green },
                { "tdoor9",      Block.tDoor_TNT },
                { "tdoor10",     Block.tDoor_Slab },
                { "tair_switch", Block.tDoor_Air },
                { "tdoor11",     Block.tDoor_Air },
                { "tdoor12",     Block.tDoor_Water },
                { "tdoor13",     Block.tDoor_Lava },
                
                { "odoor_tree", Block.oDoor_Log },
                { "odoor2",     Block.oDoor_Obsidian },
                { "odoor3",     Block.oDoor_Glass },
                { "odoor4",     Block.oDoor_Stone },
                { "odoor5",     Block.oDoor_Leaves },
                { "odoor6",     Block.oDoor_Sand },
                { "odoor7",     Block.oDoor_Wood },
                { "odoor8",     Block.oDoor_Green },
                { "odoor9",     Block.oDoor_TNT },
                { "odoor10",    Block.oDoor_Slab },
                { "odoor11",    Block.oDoor_Lava },
                { "odoor12",    Block.oDoor_Water },
                
                { "steps",          Block.Slab },
                { "double_steps",   Block.DoubleSlab },
                { "step",           Block.Slab },
                { "double_step",    Block.DoubleSlab },
                { "grey",           Block.Gray },
                { "door_darkgray",  Block.Door_Black },
                { "door_lightgray", Block.Door_Gray },
            };
        }     

        // Using a single const string reduces size by 2KB
        const string DEFAULT_NAMES =
            "Air@" +
            "Stone@" +
            "Grass@" +
            "Dirt@" +
            "Cobblestone@" +
            "Wood@" +
            "Sapling@" +
            "Bedrock@" +
            "Active_Water@" +
            "Water@" +
            "Active_Lava@" +
            "Lava@" +
            "Sand@" +
            "Gravel@" +
            "Gold_Ore@" +
            "Iron_Ore@" +
            "Coal@" +
            "Log@" +
            "Leaves@" +
            "Sponge@" +
            "Glass@" +
            "Red@" +
            "Orange@" +
            "Yellow@" +
            "Lime@" +
            "Green@" +
            "Teal@" +
            "Aqua@" +
            "Cyan@" +
            "Blue@" +
            "Indigo@" +
            "Violet@" +
            "Magenta@" +
            "Pink@" +
            "Black@" +
            "Gray@" +
            "White@" +
            "Dandelion@" +
            "Rose@" +
            "Brown_Shroom@" +
            "Red_Shroom@" +
            "Gold@" +
            "Iron@" +
            "DoubleSlab@" +
            "Slab@" +
            "Brick@" +
            "TNT@" +
            "BookShelf@" +
            "MossyRocks@" +
            "Obsidian@" +
            "CobblestoneSlab@" +
            "Rope@" +
            "SandStone@" +
            "Snow@" +
            "Fire@" +
            "LightPink@" +
            "ForestGreen@" +
            "Brown@" +
            "DeepBlue@" +
            "Turquoise@" +
            "Ice@" +
            "CeramicTile@" +
            "MagmaBlock@" +
            "Pillar@" +
            "Crate@" +
            "StoneBrick@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "FlagBase@" +
            "@" +
            "@" +
            "Fast_Hot_Lava@" +
            "C4@" +
            "C4_Det@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "Door_Cobblestone@" +
            "@" +
            "@" +
            "Door_Red@" +
            "@" +
            "Door_Orange@" +
            "Door_Yellow@" +
            "Door_LightGreen@" +
            "@" +
            "Door_AquaGreen@" +
            "Door_Cyan@" +
            "Door_LightBlue@" +
            "Door_Purple@" +
            "Door_LightPurple@" +
            "Door_Pink@" +
            "Door_DarkPink@" +
            "Door_DarkGrey@" +
            "Door_LightGrey@" +
            "Door_White@" +
            "@" +
            "Op_Glass@" +
            "Opsidian@" +
            "Op_Brick@" +
            "Op_Stone@" +
            "Op_Cobblestone@" +
            "Op_Air@" +
            "Op_Water@" +
            "Op_Lava@" +
            "@" +
            "Lava_Sponge@" +
            "Wood_Float@" +
            "Door@" +
            "Lava_Fast@" +
            "Door_Obsidian@" +
            "Door_Glass@" +
            "Door_Stone@" +
            "Door_Leaves@" +
            "Door_Sand@" +
            "Door_Wood@" +
            "Door_Green@" +
            "Door_TNT@" +
            "Door_Stair@" +
            "tDoor@" +
            "tDoor_Obsidian@" +
            "tDoor_Glass@" +
            "tDoor_Stone@" +
            "tDoor_Leaves@" +
            "tDoor_Sand@" +
            "tDoor_Wood@" +
            "tDoor_Green@" +
            "White_Message@" +
            "Black_Message@" +
            "Air_Message@" +
            "Water_Message@" +
            "Lava_Message@" +
            "tDoor_TNT@" +
            "tDoor_Stair@" +
            "tDoor_Air@" +
            "tDoor_Water@" +
            "tDoor_lava@" +
            "Waterfall@" +
            "Lavafall@" +
            "@" +
            "Water_Faucet@" +
            "Lava_Faucet@" +
            "Finite_Water@" +
            "Finite_Lava@" +
            "Finite_Faucet@" +
            "oDoor@" +
            "oDoor_Obsidian@" +
            "oDoor_Glass@" +
            "oDoor_Stone@" +
            "oDoor_Leaves@" +
            "oDoor_Sand@" +
            "oDoor_Wood@" +
            "oDoor_Green@" +
            "oDoor_TNT@" +
            "oDoor_Stair@" +
            "oDoor_Lava@" +
            "oDoor_Water@" +
            "Air_Portal@" +
            "Water_Portal@" +
            "Lava_Portal@" +
            "Custom_Block@" +
            "Air_Door@" +
            "Air_Switch@" +
            "Door_Water@" +
            "Door_Lava@" +
            "oDoor_Air@" +
            "oDoor_Obsidian_Air@" +
            "oDoor_Glass_Air@" +
            "oDoor_Stone_Air@" +
            "oDoor_Leaves_Air@" +
            "oDoor_Sand_Air@" +
            "oDoor_Wood_Air@" +
            "Blue_Portal@" +
            "Orange_Portal@" +
            "oDoor_Red@" +
            "oDoor_TNT_Air@" +
            "oDoor_Stair_Air@" +
            "oDoor_Lava_Air@" +
            "oDoor_Water_Air@" +
            "Small_TNT@" +
            "Big_TNT@" +
            "TNT_Explosion@" +
            "Lava_Fire@" +
            "Nuke_TNT@" +
            "RocketStart@" +
            "RocketHead@" +
            "Firework@" +
            "Hot_Lava@" +
            "Cold_Water@" +
            "Nerve_Gas@" +
            "Active_Cold_Water@" +
            "Active_Hot_Lava@" +
            "Magma@" +
            "Geyser@" +
            "Checkpoint@" +
            "@" +
            "@" +
            "Air_Flood@" +
            "Door_Air@" +
            "Air_Flood_Layer@" +
            "Air_Flood_Down@" +
            "Air_Flood_Up@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "Door8_Air@" +
            "Door9_Air@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "Door_Iron@" +
            "Door_Dirt@" +
            "Door_Grass@" +
            "Door_Blue@" +
            "Door_Book@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "@" +
            "Train@" +
            "Creeper@" +
            "Zombie@" +
            "Zombie_Head@" +
            "@" +
            "Dove@" +
            "Pidgeon@" +
            "Duck@" +
            "Phoenix@" +
            "Red_Robin@" +
            "Blue_Bird@" +
            "@" +
            "Killer_Phoenix@" +
            "@" +
            "@" +
            "GoldFish@" +
            "Sea_Sponge@" +
            "Shark@" +
            "Salmon@" +
            "Betta_Fish@" +
            "Lava_Shark@" +
            "Snake@" +
            "Snake_Tail@" +
            "Door_Gold@" +
            "@" +
            "@";
    }
}
