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

namespace MCGalaxy {
    
    public sealed partial class Block {
        
        public static BlockProps[] Properties = new BlockProps[256];
        public static Dictionary<string, byte> Aliases = new Dictionary<string, byte>();
        
        static void SetCoreProperties() {
            for (int i = 0; i < 256; i++)
                Properties[i] = new BlockProps((byte)i);
            for (int i = 0; i < 256; i++) {
                // Fallback for unrecognised physics blocks
                if (i >= CpeCount) Properties[i].ConvertId = Block.orange;
                
                if ((i >= op_glass && i <= op_lava) || i == Zero || i == rocketstart || i == blackrock)
                    Properties[i].OPBlock = true;
                
                if ((i >= tdoor && i <= tdoor8) || (i >= tdoor9 && i <= tdoor13))
                    Properties[i].IsTDoor = true;
                
                if (i >= MsgWhite && i <= MsgLava)
                    Properties[i].IsMessageBlock = true;
                
                if (i == blue_portal || i == orange_portal || (i >= air_portal && i <= lava_portal))
                    Properties[i].IsPortal = true;
                
                // ODoor blocks
                if (i >= odoor1 && i <= odoor7)
                    Properties[i].ODoorId = (byte)(odoor1_air + (i - odoor1));
                if (i >= odoor8 && i <= odoor12)
                    Properties[i].ODoorId = (byte)(odoor8_air + (i - odoor8));
                if (i >= odoor1_air && i <= odoor7_air) {
                    Properties[i].ODoorId = (byte)(odoor1 + (i - odoor1_air));
                    Properties[i].SaveConvertId = Properties[i].ODoorId;
                }                    
                if (i >= odoor8_air && i <= odoor12_air) {
                    Properties[i].ODoorId = (byte)(odoor8 + (i - odoor8_air));
                    Properties[i].SaveConvertId = Properties[i].ODoorId;
                }
                
                if ((i >= red && i <= white) || (i >= lightpink && i <= turquoise))
                    Properties[i].LavaKills = true;
                if (i == air || i == shrub || (i >= yellowflower && i <= redmushroom)) {
                    Properties[i].LavaKills = true;
                    Properties[i].WaterKills = true;
                }
                
                // Door blocks
                if (i >= door_obsidian && i <= door_stair)
                    Door((byte)i, (byte)(door_obsidian_air + (i - door_obsidian)));
                if (i >= door_iron && i <= door_book)
                     Door((byte)i, (byte)(door_iron_air + (i - door_iron)));
                if (i >= door_darkpink && i <= door_white)
                    Door((byte)i, (byte)(door_darkpink_air + (i - door_darkpink)));
            }
            // Other door blocks, since they aren't in a consistent order
            Door(door_tree, door_tree_air);
            Door(door_red, door_red_air);
            Door(door_cobblestone, door_cobblestone_air);
            Door(door_gold, door_gold_air);
            Door(air_door, air_door_air);
            Door(air_switch, air_switch_air);
            Door(water_door, water_door_air);
            Door(lava_door, lava_door_air);
            
            // Block specific properties
            Properties[air_flood].SaveConvertId = air;
            Properties[air_flood_down].SaveConvertId = air;
            Properties[air_flood_layer].SaveConvertId = air;
            Properties[air_flood_up].SaveConvertId = air;
            Properties[wood].LavaKills = true; Properties[trunk].LavaKills = true;
            Properties[sponge].LavaKills = true; Properties[bookcase].LavaKills = true;
            Properties[leaf].LavaKills = true;
            Properties[red].IsRails = true; Properties[op_air].IsRails = true;
            SetupDefaultNames();
        }
        
        static void Door(byte doorId, byte airId, bool saveConvert = true) {
            Properties[doorId].DoorAirId = airId;
            Properties[airId].DoorId = doorId;
            if (saveConvert)
                Properties[airId].SaveConvertId = doorId;
        }
        
        static void SetupDefaultNames() {
            string[] names = { "air", "stone", "grass", "dirt", "cobblestone", "wood", "sapling", 
                "bedrock", "active_water", "water", "active_lava", "lava", "sand", "gravel", 
                "gold_ore", "iron_ore", "coal", "log", "leaves", "sponge", "glass", "red", 
                "orange", "yellow", "lime", "green", "teal", "aqua", "cyan", 
                "blue", "indigo", "violet", "magenta", "pink", "black", "gray", "white", 
                "dandelion", "rose", "brown_shroom", "red_shroom", "gold", "iron", 
                "doubleslab", "slab", "brick", "tnt", "bookshelf", "mossyrocks", 
                "obsidian", "cobblestoneslab", "rope", "sandstone", "snow", "fire", "lightpink", 
                "forestgreen", "brown", "deepblue", "turquoise", "ice", "ceramictile", "magmablock", 
                "pillar", "crate", "stonebrick", "door_darkpink_air", "door_darkgrey_air", 
                "door_lightgrey_air", "door_white_air", "flagbase", "unknown", "unknown", 
                "fast_hot_lava", "c4", "c4_det", "unknown", "unknown", "unknown", "unknown", 
                "door_cobblestone", "door_cobblestone_air", "unknown", "door_red", "door_red_air", 
                "door_orange", "door_yellow", "door_lightgreen", "unknown", "door_aquagreen", 
                "door_cyan", "door_lightblue", "door_purple", "door_lightpurple", "door_pink", 
                "door_darkpink", "door_darkgrey", "door_lightgrey", "door_white", "unknown", 
                "op_glass", "opsidian", "op_brick", "op_stone", "op_cobblestone", "op_air", 
                "op_water", "op_lava", "griefer_stone", "lava_sponge", "wood_float", "door", 
                "lava_fast", "door_obsidian", "door_glass", "door_stone", "door_leaves", "door_sand", 
                "door_wood", "door_green", "door_tnt", "door_stair", "tdoor", "tdoor_obsidian", 
                "tdoor_glass", "tdoor_stone", "tdoor_leaves", "tdoor_sand", "tdoor_wood", 
                "tdoor_green", "white_message", "black_message", "air_message", "water_message", 
                "lava_message", "tdoor_tnt", "tdoor_stair", "tdoor_air", "tdoor_water", "tdoor_lava", 
                "waterfall", "lavafall", "unknown", "water_faucet", "lava_faucet", "finite_water", 
                "finite_lava", "finite_faucet", "odoor", "odoor_obsidian", "odoor_glass", 
                "odoor_stone", "odoor_leaves", "odoor_sand", "odoor_wood", "odoor_green", 
                "odoor_tnt", "odoor_stair", "odoor_lava", "odoor_water", "air_portal", "water_portal", 
                "lava_portal", "custom_block", "air_door", "air_switch", "door_water", "door_lava", 
                "odoor_wood_air", "odoor_obsidian_air", "odoor_glass_air", "odoor_stone_air", 
                "odoor_leaves_air", "odoor_sand_air", "odoor_wood_air", "blue_portal", "orange_portal", 
                "odoor_red", "odoor_tnt_air", "odoor_stair_air", "odoor_lava_air", "odoor_water_air", 
                "small_tnt", "big_tnt", "tnt_explosion", "lava_fire", "nuke_tnt", "rocketstart", 
                "rockethead", "firework", "hot_lava", "cold_water", "nerve_gas", "active_cold_water", 
                "active_hot_lava", "magma", "geyser", "checkpoint", "unknown", "unknown", "air_flood",
                "door_air", "air_flood_layer", "air_flood_down", "air_flood_up", "door2_air", 
                "door3_air", "door4_air", "door5_air", "door6_air", "door7_air", "door8_air", 
                "door9_air", "door10_air", "door11_air", "door12_air", "door13_air", "door14_air", 
                "unknown", "unknown", "door_iron", "door_dirt", "door_grass", "door_blue", "door_book", 
                "door_iron_air", "door_dirt_air", "door_grass_air", "door_blue_air", "door_book_air", 
                "train", "creeper", "zombie", "zombie_head", "unknown", "dove", "pidgeon", "duck", 
                "phoenix", "red_robin", "blue_bird", "unknown", "killer_phoenix", "unknown", "unknown", 
                "goldfish", "sea_sponge", "shark", "salmon", "betta_fish", "lava_shark", "snake", 
                "snake_tail", "door_gold", "door_gold_air", "unknown" };
            for (int i = 0; i < names.Length; i++) {
                Properties[i].Name = names[i];
                if (names[i] != "unknown") 
                    Aliases[names[i]] = (byte)i;               
                if (names[i].IndexOf('_') >= 0)
                    Aliases[names[i].Replace("_", "")] = (byte)i;
            }
            
            // Add old MCGalaxy names
            Aliases["purple"] = purple; Aliases["blueviolet"] = blue;
            Aliases["adminium"] = blackrock; Aliases["bookcase"] = bookcase;
            Aliases["plant"] = shrub; Aliases["mossy_cobblestone"] = stonevine;
            Aliases["springgreen"] = aquagreen; Aliases["greenyellow"] = lightgreen;
            Aliases["red_flower"] = redflower; Aliases["yellow_flower"] = yellowflower;
            Aliases["stair"] = staircasestep; Aliases["double_stair"] = staircasefull;
            // Add other aliases
            Aliases["planks"] = wood; Aliases["tree"] = trunk;
            Aliases["stairs"] = staircasestep; Aliases["slab"] = staircasestep;
            Aliases["doubleslab"] = staircasefull; Aliases["slabfull"] = staircasefull;
            Aliases["solid"] = blackrock; Aliases["admintite"] = blackrock;
            Aliases["blackrock"] = blackrock; Aliases["activewater"] = water;
            Aliases["activelava"] = lava; Aliases["fhl"] = fastdeathlava;
            Aliases["water_door"] = water_door; Aliases["lava_door"] = lava_door;
            Aliases["acw"] = activedeathwater; Aliases["ahl"] = activedeathlava;
            
            Aliases["door_tree"] = door_tree; Aliases["door2"] = door_obsidian; 
            Aliases["door3"] = door_glass; Aliases["door4"] = door_stone; 
            Aliases["door5"] = door_leaves; Aliases["door6"] = door_sand; 
            Aliases["door7"] = door_wood; Aliases["door8"] = door_green; 
            Aliases["door9"] = door_tnt; Aliases["door10"] = door_stair; 
            Aliases["door11"] = door_iron; Aliases["door12"] = door_dirt; 
            Aliases["door13"] = door_grass; Aliases["door14"] = door_blue; 
            Aliases["door15"] = door_book; Aliases["door16"] = door_gold; 
            Aliases["door17"] = door_cobblestone; Aliases["door18"] = door_red; 
            
            Aliases["tdoor_tree"] = tdoor; Aliases["tdoor2"] = tdoor2;
            Aliases["tdoor3"] = tdoor3; Aliases["tdoor4"] = tdoor4; 
            Aliases["tdoor5"] = tdoor5; Aliases["tdoor6"] = tdoor6; 
            Aliases["tdoor7"] = tdoor7; Aliases["tdoor8"] = tdoor8; 
            Aliases["tdoor9"] = tdoor9; Aliases["tdoor10"] = tdoor10; 
            Aliases["tair_switch"] = tdoor11; Aliases["tdoor11"] = tdoor11; 
            Aliases["tdoor12"] = tdoor12; Aliases["tdoor13"] = tdoor13; 
            
            Aliases["odoor_tree"] = odoor1; Aliases["odoor2"] = odoor2;
            Aliases["odoor3"] = odoor3; Aliases["odoor4"] = odoor4;
            Aliases["odoor5"] = odoor5; Aliases["odoor6"] = odoor6;
            Aliases["odoor7"] = odoor7; Aliases["odoor8"] = odoor8;
            Aliases["odoor9"] = odoor9; Aliases["odoor10"] = odoor10;
            Aliases["odoor11"] = odoor11; Aliases["odoor12"] = odoor12;
            
            Aliases["steps"] = staircasestep; Aliases["double_steps"] = staircasefull;
            Aliases["step"] = staircasestep; Aliases["double_step"] = staircasefull;
        }
    }
}
